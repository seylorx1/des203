using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    // Variables 
    private Rigidbody crabRigidbody;

    public GameObject
        thirdCam,
        firstCam,
        leftClaw,
        rightClaw;

    private GameObject currentCamera;

    private float xInput;
    private float yInput;
    private float rCloseAmount;
    private float lCloseAmount;

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

    private Quaternion
        lClawStart,
        rClawStart,
        lClawEnd,
        rClawEnd;

    private bool
        jumpAttempt = false,
        snip = false,
        onGround = false;


    void Awake() {
        //Convert euler angles to quaternion before anything else.
        //(Saves a bit of processing.)
        lClawStart = Quaternion.Euler(lClawEulerStart);
        rClawStart = Quaternion.Euler(rClawEulerStart);
        lClawEnd = Quaternion.Euler(lClawEulerEnd);
        rClawEnd = Quaternion.Euler(rClawEulerEnd);
    }

    void Start() {
        crabRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        #region Handle Inputs

        //Toggle snip mode on "Joystick1Button2".
        if (Input.GetKeyDown(KeyCode.Joystick1Button2)) {
            snip = !snip;
        }

        //Check if player attempted to jump
        if (!jumpAttempt && onGround) {
            jumpAttempt = Input.GetKeyDown(KeyCode.Joystick1Button0);
        }

        //Get inputs
        //TODO -- input manager.
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        #endregion

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

        //Get the value of the left trigger.
        float lTrigger = Input.GetAxis("Left Trigger");
        //If left trigger is pressed, ignore the sway and instead set the close amount to the axis value.
        if (lTrigger > 0.0f) {
            lCloseAmount = lTrigger;
        }

        //Set the right close amount to a cosine sway. (Same as sine wave, but with a half-interval offset. Makes it look more interesting.)
        //Values between 0 and 1.
        rCloseAmount =
            (Mathf.Cos(Time.time * 2.0f) + 1.0f) * 0.5f *   //Calculate a cosine wave between 0 and 1
            (snip ? 0.05f : 0.2f);                          //Scale the wave down based on whether snip mode is active or not.

        //Get the value of the left trigger.
        float rTrigger = Input.GetAxis("Right Trigger");
        //If left trigger is pressed, ignore the sway and instead set the close amount to the axis value.
        if (rTrigger > 0.0f) {
            rCloseAmount = rTrigger;
        }

        //Interpolate between start and end rotations based on the close amounts.

        leftClaw.transform.localRotation = Quaternion.Lerp(
            lClawStart,
            lClawEnd,
            lCloseAmount);
        rightClaw.transform.localRotation = Quaternion.Lerp(
            rClawStart,
            rClawEnd,
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
        if (Mathf.Abs(xInput) > 0.1f) { //Accomodate for stick-drift
            Vector3 targetVelocity = transform.right * -xInput * speed;

            Vector3 velocityChange = (targetVelocity - crabRigidbody.velocity);
            velocityChange.x = Mathf.Clamp(targetVelocity.x, -maxSpeedChange, maxSpeedChange);
            velocityChange.z = Mathf.Clamp(targetVelocity.z, -maxSpeedChange, maxSpeedChange);
            velocityChange.y = 0;

            crabRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        //YInput is active
        if (yInput != 0) {
            crabRigidbody.rotation = Quaternion.Euler(
                crabRigidbody.rotation.eulerAngles.x,
                crabRigidbody.rotation.eulerAngles.y + yInput * Time.fixedDeltaTime * rotateSpeed,
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

    }


    void OnCollisionEnter(Collision collision) {

        //TODO check collision *starts on* ground

        onGround = true;
    }

    void OnCollisionExit(Collision collision) {

        //TODO check collision *left* ground

        onGround = false;
    }

}
