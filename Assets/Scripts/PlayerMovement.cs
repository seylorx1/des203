using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables 
    private Rigidbody rigbod;
    public GameObject thirdCam;
    public GameObject firstCam;
    public GameObject leftLegs;
    public GameObject rightLegs;
    public GameObject leftClaw;
    public GameObject rightClaw;
    private float xInput;
    private float yInput;
    private float rTrigger;
    private float lTrigger;
    public float speed = 5;
    public float jumpForce = 4;
    public float clawSpeed = 5;
    public float resetSpeed = 0.1f;
    public bool snip;
    public bool onGround;
    private bool hasExecuted = false;
    private Quaternion rClawDefault;
    public Quaternion lClawDefault;

    void Start()
    {
        rigbod = GetComponent<Rigidbody>();
        Invoke("DefaultSnipRotation", 1);
    }

    // Update is called once per frame
    void Update()
    {
        // Control switch
        if (Input.GetKeyDown(KeyCode.Joystick1Button2) && snip == false)
        { 
            snip = true;
            Debug.Log("Snip");
        }

        else if (Input.GetKeyDown(KeyCode.JoystickButton2) && snip == true)
        {
            snip = false;
            Debug.Log("NoSnip");
        }
 
        // Get input
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        rTrigger = Input.GetAxis("Right Trigger");
        lTrigger = Input.GetAxis("Left Trigger");

        //Movement
        if (snip == false) 
            {
            MoveMode();
            thirdCam.gameObject.SetActive(true);
            firstCam.gameObject.SetActive(false);
        }

        else if (snip == true)
            {
            SnipMode();
            firstCam.gameObject.SetActive(true);
            thirdCam.gameObject.SetActive(false);
            }
        
        if (Input.GetKeyDown(KeyCode.Joystick1Button0) && onGround == true)
        {
            rigbod.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            onGround = false;
        }
    }

    // Movement mode
    void MoveMode()
    {
        transform.Translate(Vector3.right * speed * 2 * Time.deltaTime * xInput);
        transform.Translate(Vector3.forward * speed * Time.deltaTime * yInput);

        if (hasExecuted == true)
        {
            leftClaw.transform.Rotate(Vector3.up * Time.deltaTime * clawSpeed * lTrigger);
            rightClaw.transform.Rotate(Vector3.down * Time.deltaTime * clawSpeed * rTrigger);


            if (lTrigger <= 0.1)
            {
                leftClaw.transform.rotation = Quaternion.Slerp(transform.rotation, lClawDefault, resetSpeed * Time.time);
            }

            if (rTrigger <= 0.1)
            {
                rightClaw.transform.rotation = Quaternion.Slerp(transform.rotation, rClawDefault, resetSpeed * Time.time);
            }

        }        
    }


    // Snip mode
    void SnipMode()
    {
        
        
    }

    // Ground Check
    void OnCollisionEnter(Collision collision)
    {
        onGround = true;
    }

    void DefaultSnipRotation()
    {
        rClawDefault = rightClaw.transform.rotation;
        lClawDefault = leftClaw.transform.rotation;
        hasExecuted = true;
    }
}
