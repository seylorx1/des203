using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
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
        //Convert euler angles to quaternion.
        //(Saves a bit of processing.)
        lClawStart = Quaternion.Euler(lClawEulerStart);
        rClawStart = Quaternion.Euler(rClawEulerStart);
        lClawEnd = Quaternion.Euler(lClawEulerEnd);
        rClawEnd = Quaternion.Euler(rClawEulerEnd);
    }

    void Start() {
        crabRigidbody = GetComponent<Rigidbody>();

        //lClawStart = leftClaw.transform.rotation;
        //rClawStart = rightClaw.transform.rotation;
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

        lCloseAmount = (Mathf.Sin(Time.time * 2.0f) + 1.0f) * 0.5f * (snip ? 0.05f : 0.2f);
        float lTrigger = Input.GetAxis("Left Trigger");
        if (lTrigger > 0.0f) {
            lCloseAmount = lTrigger;
        }

        rCloseAmount = (Mathf.Cos(Time.time * 2.0f) + 1.0f) * 0.5f * (snip ? 0.05f : 0.2f);
        float rTrigger = Input.GetAxis("Right Trigger");
        if (rTrigger > 0.0f) {
            rCloseAmount = rTrigger;
        }


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

}
