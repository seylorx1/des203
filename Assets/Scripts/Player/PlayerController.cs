using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour {
    #region Variables

    #region Public
    public SkinnedMeshRenderer[] crabMeshRenderers;
    public SkinnedMeshRenderer crabEyes;

    public GameObject
        flipindicator,
        modeindicator,
        secondPersonCamera,
        firstPersonCamera,
        thirdPersonCamera,
        startCam,
        leftClaw,
        rightClaw,
        lClawIKTarget,
        rClawIKTarget;

    public Transform leftEar, rightEar;

    public float
        Acceleration = 5,
        MaxVelocity = 5,
        rotateSpeed = 20.0f,
        jumpForce = 4,
        flipTorque = 0.2f,
        lTrigger,
        rTrigger;

    public int
        Deaths = 0;

    public Vector2
        inputLS,
        inputRS;

    public bool camToggle = false;

    [System.Serializable]
    public struct CrabClawData {
        public Vector3
            lClawEulerStart,
            rClawEulerStart,
            lClawEulerEnd,
            rClawEulerEnd;

        public Vector2
            lClawIKMin,
            lClawIKMax,
            rClawIKMin,
            rClawIKMax;

        public float
            lClawIK_Z,
            rClawIK_Z,
            clawSpeed;
    }
    public CrabClawData crabClawData;
    #endregion

    #region Private
    private List<Material> crabMaterials = new List<Material>();
    private Material crabEyesMaterial;

    private FlamesPP cameraFlamesPP;
    private Vignette cameraFlamesVignette;

    private Collider crabCollider;
    private Rigidbody crabRigidbody;

    private Animation modeindicatoranim;

    private Quaternion
    lClawQuatStart,
    rClawQuatStart,
    lClawQuatEnd,
    rClawQuatEnd;

    private float
        lCloseAmount,
        rCloseAmount;

    private int layerMask_Player;

    private bool
        anyKeyPress = false, // Workaround for the input system not having an easy way to check if any key is pressed 
        jumpAttempt = false,
        onGround = false, // Is the crab touching the ground? (Used to prevent spam jumping / b-hopping.)
        isOnEdge = false; // Is the crab on their side? (Prevents peculiar wall bug.)
    #endregion

    #region Properties
    //Used externally.
    public bool Snip { get; private set; } = false;

    public Vector2 LookAxis {
        get {
            if (!Snip) {
                return inputRS;
            }
            return Vector2.zero;
        }
    }

    public bool LeftClawPivot { get; private set; } = false;
    public bool RightClawPivot { get; private set; } = false;

    // Is the crab flipped over? (Used for flipping.)
    public bool CrabFlipped { get; private set; } = false;

    public float Heat { get; private set; } = 0;

    private float _currentCam;

    /// <summary>
    /// Updates the current camera.
    /// </summary>
    public float CurrentCam {
        get {
            return _currentCam;
        }
        private set {
            _currentCam = Mathf.Clamp(value, 0, 2);

            //Update cameras.
            thirdPersonCamera.gameObject.SetActive(_currentCam == 0);
            firstPersonCamera.gameObject.SetActive(_currentCam == 1);
            secondPersonCamera.gameObject.SetActive(_currentCam == 2);
        }
    }

    #endregion

    #endregion

    void Awake() {
        //Convert euler angles to quaternion before anything else.
        //(Saves a bit of processing.)
        lClawQuatStart = Quaternion.Euler(crabClawData.lClawEulerStart);
        rClawQuatStart = Quaternion.Euler(crabClawData.rClawEulerStart);
        lClawQuatEnd = Quaternion.Euler(crabClawData.lClawEulerEnd);
        rClawQuatEnd = Quaternion.Euler(crabClawData.rClawEulerEnd);

        //detatch transform from parent P_Crab.
        transform.parent = null;
    }

    void Start() {
        RegisterInputEvents();

        #region Obtain Material Instance References
        foreach (SkinnedMeshRenderer r in crabMeshRenderers) {
            Material rMat = r.material;

            if (!crabMaterials.Contains(rMat) && rMat.HasProperty("_OutlineColor")) {
                crabMaterials.Add(rMat);
            }
        }
        crabEyesMaterial = crabEyes.material;
        #endregion

        PostProcessVolume cameraPostProcessVolume = Camera.main.GetComponentInChildren<PostProcessVolume>();
        if (cameraPostProcessVolume != null) {
            cameraPostProcessVolume.profile.TryGetSettings(out cameraFlamesPP);
            cameraPostProcessVolume.profile.TryGetSettings(out cameraFlamesVignette);
        }

        crabCollider = GetComponent<Collider>();
        crabRigidbody = GetComponent<Rigidbody>();
        modeindicatoranim = modeindicator.GetComponent<Animation>();

        layerMask_Player = LayerMask.GetMask("PlayerCharacter");

        CurrentCam = 0;
    }

    // Update is called once per frame
    void Update() {
        if (anyKeyPress) {
            startCam.gameObject.SetActive(false);
        }

        if (Snip) {
            SnipMode();
        }

        #region Crab Transparency
        {
            float crabTransparency =
                CurrentCam == 0 ?
                Mathf.Clamp(Vector3.SqrMagnitude(transform.position - Camera.main.transform.position) - 0.75f, 0.25f, 1.0f) :
                1.0f;

            foreach (Material rMat in crabMaterials) {
                Color rMatColor = rMat.color;
                rMatColor.a = crabTransparency;
                rMat.color = rMatColor;

                if(crabTransparency == 1.0f) {
                    rMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    rMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    rMat.SetOverrideTag("RenderType", "Opaque");
                    rMat.renderQueue = 2000;
                }
                else {
                    rMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    rMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    rMat.SetOverrideTag("RenderType", "Transparent");
                    rMat.renderQueue = 3001;
                }
            }

            Color crabEyesMaterialColor = crabEyesMaterial.color;
            crabEyesMaterialColor.a = crabTransparency;
            crabEyesMaterial.color = crabEyesMaterialColor;

            if (crabTransparency == 1.0f) {
                //Sets opaque
                if (crabEyesMaterial.renderQueue != 2000) { //Only want to set all this shit on one frame.

                    crabEyesMaterial.SetFloat("_Mode", 0); //Sets the mode. mostly trivial.

                    crabEyesMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    crabEyesMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    crabEyesMaterial.SetInt("_ZWrite", 1);
                    crabEyesMaterial.DisableKeyword("_ALPHATEST_ON");
                    crabEyesMaterial.DisableKeyword("_ALPHABLEND_ON");
                    crabEyesMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    crabEyesMaterial.renderQueue = 2000;
                }
            }
            else {
                //Sets "fade"
                if (crabEyesMaterial.renderQueue != 3001) { //Only want to set all this shit on one frame.

                    crabEyesMaterial.SetFloat("_Mode", 2); //Sets the mode. mostly trivial.

                    crabEyesMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    crabEyesMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    crabEyesMaterial.SetInt("_ZWrite", 0);
                    crabEyesMaterial.DisableKeyword("_ALPHATEST_ON");
                    crabEyesMaterial.EnableKeyword("_ALPHABLEND_ON");
                    crabEyesMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    crabEyesMaterial.renderQueue = 3001;
                }
            }

        }
        #endregion

        #region Crab Claw Controls
        {
            //Set the left close amount to a sine sway.
            //Values between 0 and 1.
            lCloseAmount =
                (Mathf.Sin(Time.time * 2.0f) + 1.0f) * 0.5f *   //Calculate a sine wave between 0 and 1
                (Snip ? 0.05f : 0.2f);                          //Scale the wave down based on whether snip mode is active or not.

            //If left trigger is pressed, ignore the sway and instead set the close amount to the axis value.
            if (lTrigger > 0.0f) {
                lCloseAmount = lTrigger;
            }

            //Set the right close amount to a cosine sway. (Same as sine wave, but with a half-interval offset. Makes it look more interesting.)
            //Values between 0 and 1.
            rCloseAmount =
                (Mathf.Cos(Time.time * 2.0f) + 1.0f) * 0.5f *   //Calculate a cosine wave between 0 and 1
                (Snip ? 0.05f : 0.2f);                          //Scale the wave down based on whether snip mode is active or not.

            //If left trigger is pressed, ignore the sway and instead set the close amount to the axis value.
            if (rTrigger > 0.0f) {
                rCloseAmount = rTrigger;
            }

            //Interpolate between start and end rotations based on the close amounts.
            leftClaw.transform.localRotation = Quaternion.Lerp(
                lClawQuatStart,
                lClawQuatEnd,
                lCloseAmount);

            rightClaw.transform.localRotation = Quaternion.Lerp(
                rClawQuatStart,
                rClawQuatEnd,
                rCloseAmount);
        }
        #endregion

    }

    void FixedUpdate() {
        //Shoot a very short spehere cast from the centre of the collider to just outside the collider's bounds to test if the crab is on a surface.
        onGround = Physics.SphereCast(
            new Ray(crabCollider.bounds.center, Vector3.down),
            0.1f,
            crabCollider.bounds.extents.y + 0.1f,
            ~layerMask_Player //The tilde inverts the integer bits, meaning that it will collide against everything BUT the player character :)
            );

        float crabVerticalDot = Vector3.Dot(Vector3.up, transform.up); //Provides information about the orientation of the crab.

        //Detect whether the crab is tipped over.
        CrabFlipped = crabVerticalDot < 0.0f;
        if (CrabFlipped && anyKeyPress) {
            //Reset the camera if flipped.
            CurrentCam = 0;
        }

        //Detect if the crab is lying on their side.
        isOnEdge = Mathf.Abs(crabVerticalDot) < 0.1f;

        if (!Snip) {
            MoveMode();
        }
    }

    /// <summary>
    /// Called on the physics step via FixedUpdate()
    /// </summary>
    void MoveMode() {

        //Only apply movements if the crab is touching the ground.
        
        if (onGround) {

            flipindicator.SetActive(true);

            // Ensure the crab is not flipped over or on their edge.
            if (!(CrabFlipped || isOnEdge)) {

                flipindicator.SetActive(false);

                //Apply force.
                //XInput is active.
                if (Mathf.Abs(inputLS.x) > 0.1f) { //Accomodate for stick-drift
                    Vector3 targetVelocity = transform.right * -inputLS.x * Acceleration;

                    Vector3 velocityChange = (targetVelocity - crabRigidbody.velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -MaxVelocity, MaxVelocity);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -MaxVelocity, MaxVelocity);

                    velocityChange.y = 0.15f; //Adding a little bit of lift stops the crab from clipping the ground and tipping *as much*.

                    crabRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
                }

                //Rotate
                //YInput is active
                if (Mathf.Abs(inputLS.y) > 0.1f) { //Accomodate for stick-drift
                    crabRigidbody.rotation = Quaternion.Euler(
                        crabRigidbody.rotation.eulerAngles.x,
                        crabRigidbody.rotation.eulerAngles.y + inputLS.y * Time.fixedDeltaTime * rotateSpeed,
                        crabRigidbody.rotation.eulerAngles.z);
                }
            }

            //Jump
            if (jumpAttempt) {

                if (isOnEdge || CrabFlipped) {
                    //apply torque to crab
                    crabRigidbody.AddRelativeTorque(
                        new Vector3(
                            0.0f, //Randomly add torque in one direction or an other to accomodate for edge cases.
                            0.0f,
                            Random.Range(0, 2) == 0 ? -flipTorque : flipTorque),
                        ForceMode.Impulse);
                }

                if (CrabFlipped) {
                    //attempt to rectify crab
                    crabRigidbody.AddForce(-transform.up * jumpForce, ForceMode.Impulse);
                }
                else {
                    //perform jump
                    crabRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    onGround = false;
                }

                jumpAttempt = false;
            }
        }
    }

    void SnipMode() {


        if (!LeftClawPivot) {
            if (Mathf.Abs(inputLS.x) > 0.1f || Mathf.Abs(inputLS.y) > 0.1f) { //Accomodate for stick-drift
                lClawIKTarget.transform.position +=
                    ((Camera.main.transform.right * inputLS.x) + (Camera.main.transform.up * inputLS.y)) *
                    crabClawData.clawSpeed * Time.deltaTime;

                lClawIKTarget.transform.localPosition = new Vector3(
                    Mathf.Clamp(lClawIKTarget.transform.localPosition.x, crabClawData.lClawIKMin.x, crabClawData.lClawIKMax.x),
                    Mathf.Clamp(lClawIKTarget.transform.localPosition.y, crabClawData.lClawIKMin.y, crabClawData.lClawIKMax.y),
                    crabClawData.lClawIK_Z
                    );
            }
        }

        if (!RightClawPivot) {
            if (Mathf.Abs(inputRS.x) > 0.1f || Mathf.Abs(inputRS.y) > 0.1f) { //Accomodate for stick-drift
                rClawIKTarget.transform.position +=
                    ((Camera.main.transform.right * inputRS.x) + (Camera.main.transform.up * inputRS.y)) *
                    crabClawData.clawSpeed * Time.deltaTime;

                rClawIKTarget.transform.localPosition = new Vector3(
                    Mathf.Clamp(rClawIKTarget.transform.localPosition.x, crabClawData.rClawIKMin.x, crabClawData.rClawIKMax.x),
                    Mathf.Clamp(rClawIKTarget.transform.localPosition.y, crabClawData.rClawIKMin.y, crabClawData.rClawIKMax.y),
                    crabClawData.rClawIK_Z
                    );
            }
        }
    }

    /// <summary>
    /// Damage and heat are synonymous in CCR.
    /// </summary>
    /// <param name="amount">Amount of "heat" taken.</param>
    public void TakeHeat(float amount) {
        Heat += amount;

        if (Heat >= 100.0f) {
            //Crab death sequence
            Deaths = Deaths + 1;
            SceneManager.LoadScene("MainScene"); // TODO Handle death logic better.
        }

        float clampedHeat = Mathf.Clamp01(Heat * 0.01f);

        if (cameraFlamesPP != null) {
            cameraFlamesPP.flameAmount.value = clampedHeat;
            cameraFlamesVignette.intensity.value = clampedHeat * 0.6f;
        }

        foreach (Material rMat in crabMaterials) {
            Color oldColor = rMat.GetColor("_Color");
            rMat.SetColor("_Color", Color.Lerp(new Color(1.0f, 1.0f, 1.0f, oldColor.a), new Color(1.0f, 0.0f, 0.0f, oldColor.a), clampedHeat * 0.5f));
            rMat.SetColor("_OutlineColor", Color.Lerp(Color.black, Color.red, clampedHeat));
        }
    }

    #region Handle Inputs

    private void RegisterInputEvents() {

        InputSingleton inputSingleton = SingletonManager.Instance.GetSingleton<InputSingleton>();
        if (inputSingleton == null) {
            Debug.LogError("Please ensure a singleton asset is in the scene with an InputSingleton attached!");
            return;
        }

        inputSingleton.movement.performed += OnInputMovement;
        inputSingleton.movement.canceled += OnInputMovement;

        inputSingleton.look.performed += OnInputLook;
        inputSingleton.look.canceled += OnInputLook;

        inputSingleton.jump.performed += OnInputJump;
        inputSingleton.jump.canceled += OnInputJump;

        inputSingleton.snipModeToggle.performed += OnInputSnipModeToggle;
        inputSingleton.snipModeToggle.canceled += OnInputSnipModeToggle;

        inputSingleton.leftCrabClaw.performed += OnInputLeftCrabClaw;
        inputSingleton.leftCrabClaw.canceled += OnInputLeftCrabClaw;

        inputSingleton.rightCrabClaw.performed += OnInputRightCrabClaw;
        inputSingleton.rightCrabClaw.canceled += OnInputRightCrabClaw;

        inputSingleton.pivotLeftCrabClaw.performed += OnInputPivotLeftCrabClaw;
        inputSingleton.pivotLeftCrabClaw.canceled += OnInputPivotLeftCrabClaw;

        inputSingleton.pivotRightCrabClaw.performed += OnInputPivotRightCrabClaw;
        inputSingleton.pivotRightCrabClaw.canceled += OnInputPivotRightCrabClaw;

        inputSingleton.camToggle.performed += OnInputCamToggle;
        inputSingleton.camToggle.canceled += OnInputCamToggle;
    }

    private void OnInputMovement(InputAction.CallbackContext ctx) {
        inputLS = ctx.ReadValue<Vector2>();
        if (!anyKeyPress) {
            anyKeyPress = inputLS.sqrMagnitude > 0.0f;
        }
    }

    private void OnInputLook(InputAction.CallbackContext ctx) {
        inputRS = ctx.ReadValue<Vector2>();
        if (!anyKeyPress) {
            anyKeyPress = inputRS.sqrMagnitude > 0.0f;
        }
    }

    private void OnInputJump(InputAction.CallbackContext ctx) {
        //Check if player attempted to jump.
        //Player must be out of snip mode and touching the ground
        if (!Snip && !jumpAttempt && onGround) {
            anyKeyPress = true;
            jumpAttempt = ctx.ReadValueAsButton();
        }
    }

    private void OnInputSnipModeToggle(InputAction.CallbackContext ctx) {
        //Toggle snip mode on "Joystick1Button2".
        if (ctx.ReadValueAsButton()) {
            anyKeyPress = true;

            if (CrabFlipped) {
                Snip = false;
            }
            else {

                Snip = !Snip;

                if (Snip) {
                    //Set snip camera to first person.
                    //(This will not prevent the player from switching to a different camera view should they wish to.)
                    CurrentCam = 1;
                    modeindicatoranim.Play("SnipFade");
                }
                else {
                    //Resets the camera to third person.
                    //(This will not prevent the player from switching to a different camera view should they wish to.)
                    CurrentCam = 0;
                    modeindicatoranim.Play("MoveFade");
                }
            }
        }
    }

    private void OnInputLeftCrabClaw(InputAction.CallbackContext ctx) {
        lTrigger = ctx.ReadValue<float>();
    }

    private void OnInputRightCrabClaw(InputAction.CallbackContext ctx) {
        rTrigger = ctx.ReadValue<float>();
    }

    private void OnInputPivotLeftCrabClaw(InputAction.CallbackContext ctx) {
        LeftClawPivot = !LeftClawPivot;
    }

    private void OnInputPivotRightCrabClaw(InputAction.CallbackContext ctx) {
        RightClawPivot = !RightClawPivot;
    }

    private void OnInputCamToggle(InputAction.CallbackContext ctx) {
        if (ctx.ReadValueAsButton() && !CrabFlipped) {
            if (!anyKeyPress) {
                anyKeyPress = true;
            }
            else {
                CurrentCam = CurrentCam == 2 ? 0 : CurrentCam + 1;
            }
        }
    }
    #endregion
}
