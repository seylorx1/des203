using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    // Variables 
    private Rigidbody crabRigidbody;

    public GameObject
        //crabModel,
        thirdCam,
        firstCam,
        leftClaw,
        rightClaw,
        lClawIKTarget,
        rClawIKTarget;

    private Vector2
        inputLS,
        inputRS;

    private float
        lTrigger,
        rTrigger,
        lCloseAmount,
        rCloseAmount;

    public float speed = 5;
    public float maxSpeedChange = 5;

    public float rotateSpeed = 20.0f;

    public float jumpForce = 4;
    public float clawSpeed = 5;
    public float resetSpeed = 0.1f;

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
        rClawIK_Z;

    private Quaternion
        lClawQuatStart,
        rClawQuatStart,
        lClawQuatEnd,
        rClawQuatEnd;

    private bool
        jumpAttempt = false,
        snip = false,
        onGround = false;

    public Vector2 LookAxis {
        get {
            if (!snip) {
                return inputRS;
            }
            return Vector2.zero;
        }
    }


    void Awake() {
        //Convert euler angles to quaternion before anything else.
        //(Saves a bit of processing.)
        lClawQuatStart = Quaternion.Euler(lClawEulerStart);
        rClawQuatStart = Quaternion.Euler(rClawEulerStart);
        lClawQuatEnd = Quaternion.Euler(lClawEulerEnd);
        rClawQuatEnd = Quaternion.Euler(rClawEulerEnd);
    }

    void Start() {
        crabRigidbody = GetComponent<Rigidbody>();

        if (!InputManager.IsLoaded) {
            Debug.LogError("Please ensure a singleton asset is in the scene with an InputManager attached!");
            return;
        }

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
        if (!snip) {
            MoveMode();
        }
    }

    /// <summary>
    /// Called on the physics step via FixedUpdate()
    /// </summary>
    void MoveMode() {
        //Apply force.

        //XInput is active.
        if (Mathf.Abs(inputLS.x) > 0.1f) { //Accomodate for stick-drift
            Vector3 targetVelocity = transform.right * -inputLS.x * speed;

            Vector3 velocityChange = (targetVelocity - crabRigidbody.velocity);
            velocityChange.x = Mathf.Clamp(targetVelocity.x, -maxSpeedChange, maxSpeedChange);
            velocityChange.z = Mathf.Clamp(targetVelocity.z, -maxSpeedChange, maxSpeedChange);
            velocityChange.y = 0;

            crabRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        //YInput is active
        if (Mathf.Abs(inputLS.y) > 0.1f) { //Accomodate for stick-drift
            crabRigidbody.rotation = Quaternion.Euler(
                crabRigidbody.rotation.eulerAngles.x,
                crabRigidbody.rotation.eulerAngles.y + inputLS.y * Time.fixedDeltaTime * rotateSpeed,
                crabRigidbody.rotation.eulerAngles.z);
        }

        //Jump
        if (jumpAttempt && onGround) {

            crabRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpAttempt = false;
            onGround = false;
        }
    }


    void SnipMode() {

        //TODO Arms appear to have collisions enabled. Consider calling this from FixedUpdate() if this is intentional.

        if (Mathf.Abs(inputLS.x) > 0.1f || Mathf.Abs(inputLS.y) > 0.1f) { //Accomodate for stick-drift
            lClawIKTarget.transform.position +=
            ((Camera.main.transform.right * inputLS.x) +
            (Camera.main.transform.up * inputLS.y)) *
            clawSpeed * Time.deltaTime;

            lClawIKTarget.transform.localPosition = new Vector3(
                Mathf.Clamp(lClawIKTarget.transform.localPosition.x, lClawIKMin.x, lClawIKMax.x),
                Mathf.Clamp(lClawIKTarget.transform.localPosition.y, lClawIKMin.y, lClawIKMax.y),
                lClawIK_Z
                );
        }

        if (Mathf.Abs(inputRS.x) > 0.1f || Mathf.Abs(inputRS.y) > 0.1f) { //Accomodate for stick-drift
            rClawIKTarget.transform.position +=
            ((Camera.main.transform.right * inputRS.x) +
            (Camera.main.transform.up * inputRS.y)) *
            clawSpeed * Time.deltaTime;

            rClawIKTarget.transform.localPosition = new Vector3(
                Mathf.Clamp(rClawIKTarget.transform.localPosition.x, rClawIKMin.x, rClawIKMax.x),
                Mathf.Clamp(rClawIKTarget.transform.localPosition.y, rClawIKMin.y, rClawIKMax.y),
                rClawIK_Z
                );
        }
    }


    void OnCollisionEnter(Collision collision) {

        //TODO check collision *starts on* ground

        onGround = true;
    }

    void OnCollisionExit(Collision collision) {

        //TODO check collision *left* ground

        onGround = false;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Collectable") {
            Debug.Log("Item Picked Up");
            Destroy(other.gameObject);
        }
    }

    #region Handle Inputs

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
            snip = !snip;
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
