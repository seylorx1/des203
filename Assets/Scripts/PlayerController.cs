using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    // Variables 
    private Rigidbody crabRigidbody;

    public GameObject
        thirdCam,
        firstCam,
        //leftLegs,
        //rightLegs,
        leftClaw,
        rightClaw;

    private float xInput;
    private float yInput;
    private float rCloseAmount;
    private float lCloseAmount;

    public float speed = 5;
    public float jumpForce = 4;
    public float clawSpeed = 5;
    public float resetSpeed = 0.1f;
    public bool snip;
    public bool onGround;

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
            Debug.Log(snip ? "Snip Mode" : "Move Mode");
        }

        //Get inputs
        //TODO -- input manager.
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        #endregion

        if (snip) {
            SnipMode();
        }
        else {
            MoveMode();
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

    // Movement mode
    void MoveMode() {
        //Apply force.
        transform.Translate(Vector3.right * speed * 2 * Time.deltaTime * xInput);
        transform.Translate(Vector3.forward * speed * Time.deltaTime * yInput);

        //Jump
        if (Input.GetKeyDown(KeyCode.Joystick1Button0) && onGround == true) {
            crabRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            onGround = false;
        }
    }


    // Snip mode
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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable")
        {
            Debug.Log("Item Picked Up");
            Destroy(other.gameObject);
        }
    }

}
