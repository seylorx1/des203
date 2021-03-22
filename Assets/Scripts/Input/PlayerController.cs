using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour {
    #region Variables

    #region Public
    public GameObject
        thirdCam,
        firstCam,
        leftClaw,
        rightClaw,
        lClawIKTarget,
        rClawIKTarget;

    public float
        Acceleration = 5,
        MaxVelocity = 5,
        rotateSpeed = 20.0f,
        jumpForce = 4,
        flipTorque = 0.2f,
        lTrigger,
        rTrigger;

    public Vector2
        inputLS,
        inputRS;

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
    private Collider crabCollider;
    private Rigidbody crabRigidbody;

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
        jumpAttempt = false,
        snip = false,
        onGround = false, // Is the crab touching the ground? (Used to prevent spam jumping / b-hopping.)
        isOnEdge = false; // Is the crab on their side? (Prevents peculiar wall bug.)
    #endregion

    #region Properties
    //Used externally.
    public Vector2 LookAxis {
        get {
            if (!snip) {
                return inputRS;
            }
            return Vector2.zero;
        }
    }

    // Is the crab flipped over? (Used for flipping.)
    public bool CrabFlipped { get; private set; } = false;

    public float Heat { get; private set; } = 0;
    #endregion

    #endregion


    void Awake() {
        //Convert euler angles to quaternion before anything else.
        //(Saves a bit of processing.)
        lClawQuatStart = Quaternion.Euler(crabClawData.lClawEulerStart);
        rClawQuatStart = Quaternion.Euler(crabClawData.rClawEulerStart);
        lClawQuatEnd = Quaternion.Euler(crabClawData.lClawEulerEnd);
        rClawQuatEnd = Quaternion.Euler(crabClawData.rClawEulerEnd);
    }

    void Start() {
        crabCollider = GetComponent<Collider>();
        crabRigidbody = GetComponent<Rigidbody>();

        layerMask_Player = LayerMask.GetMask("PlayerCharacter");

        if (!InputManager.IsLoaded) {
            Debug.LogError("Please ensure a singleton asset is in the scene with an InputManager attached!");
            return;
        }

        registerInputEvents();
    }

    // Update is called once per frame
    void Update() {

        if (snip) {
            SnipMode();
        }

        firstCam.gameObject.SetActive(snip);
        thirdCam.gameObject.SetActive(!snip);

        #region Crab Claw Controls

        //Set the left close amount to a sine sway.
        //Values between 0 and 1.
        lCloseAmount =
            (Mathf.Sin(Time.time * 2.0f) + 1.0f) * 0.5f *   //Calculate a sine wave between 0 and 1
            (snip ? 0.05f : 0.2f);                          //Scale the wave down based on whether snip mode is active or not.

        //If left trigger is pressed, ignore the sway and instead set the close amount to the axis value.
        if (lTrigger > 0.0f) {
            lCloseAmount = lTrigger;
        }

        //Set the right close amount to a cosine sway. (Same as sine wave, but with a half-interval offset. Makes it look more interesting.)
        //Values between 0 and 1.
        rCloseAmount =
            (Mathf.Cos(Time.time * 2.0f) + 1.0f) * 0.5f *   //Calculate a cosine wave between 0 and 1
            (snip ? 0.05f : 0.2f);                          //Scale the wave down based on whether snip mode is active or not.

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

        //Detect if the crab is lying on their side.
        isOnEdge = Mathf.Abs(crabVerticalDot) < 0.1f;

        if (!snip) {
            MoveMode();
        }
    }

    /// <summary>
    /// Called on the physics step via FixedUpdate()
    /// </summary>
    void MoveMode() {

        //Only apply movements if the crab is touching the ground.
        if (onGround) {

            // Ensure the crab is not flipped over or on their edge.
            if (!(CrabFlipped || isOnEdge)) {

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

                if (CrabFlipped) {
                    //attempt to rectify crab
                    crabRigidbody.AddTorque(
                        new Vector3(
                            Random.Range(0, 2) == 0 ? -flipTorque : flipTorque, //Randomly add torque in one direction or an other to accomodate for edge cases.
                            0.0f,
                            0.0f),
                        ForceMode.Impulse);
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

        //TODO Arms appear to have collisions enabled. Consider calling this from FixedUpdate() if this is intentional.

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

    /// <summary>
    /// Damage and heat are synonymous in CCR.
    /// </summary>
    /// <param name="amount">Amount of "heat" taken.</param>
    public void TakeHeat(float amount) {
        Heat += amount;

        if (Heat >= 100.0f) {
            //Crab death sequence
            SceneManager.LoadScene("MainScene"); // TODO Handle death logic better.
        }
    }

    #region Handle Inputs

    private void registerInputEvents() {
        InputManagerData data = (InputManagerData)InputManager.Instance.SingletonBaseRef.Data;

        data.movement.performed += onInputMovement;
        data.movement.canceled += onInputMovement;

        data.look.performed += onInputLook;
        data.look.canceled += onInputLook;

        data.jump.performed += onInputJump;
        data.jump.canceled += onInputJump;

        data.snipModeToggle.performed += onInputSnipModeToggle;
        data.snipModeToggle.canceled += onInputSnipModeToggle;

        data.leftCrabClaw.performed += onInputLeftCrabClaw;
        data.leftCrabClaw.canceled += onInputLeftCrabClaw;

        data.rightCrabClaw.performed += onInputRightCrabClaw;
        data.rightCrabClaw.canceled += onInputRightCrabClaw;
    }

    private void onInputMovement(InputAction.CallbackContext ctx) {
        inputLS = ctx.ReadValue<Vector2>();
    }

    private void onInputLook(InputAction.CallbackContext ctx) {
        inputRS = ctx.ReadValue<Vector2>();
    }

    private void onInputJump(InputAction.CallbackContext ctx) {
        //Check if player attempted to jump.
        //Player must be out of snip mode and touching the ground
        if (!snip && !jumpAttempt && onGround) {
            jumpAttempt = ctx.ReadValueAsButton();
        }
    }

    private void onInputSnipModeToggle(InputAction.CallbackContext ctx) {
        //Toggle snip mode on "Joystick1Button2".
        if (ctx.ReadValueAsButton()) {
            if (CrabFlipped) {
                snip = false;
            }
            else {
                snip = !snip;
            }
        }
    }

    private void onInputLeftCrabClaw(InputAction.CallbackContext ctx) {
        lTrigger = ctx.ReadValue<float>();
    }

    private void onInputRightCrabClaw(InputAction.CallbackContext ctx) {
        rTrigger = ctx.ReadValue<float>();
    }

    #endregion
}
