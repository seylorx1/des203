using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables 
    private Rigidbody rigbod;
    private float xInput;
    private float yInput;
    public float speed = 5;
    public float jumpForce = 4;
    public bool snip;
    public bool onGround;

    void Start()
    {
        rigbod = GetComponent<Rigidbody>();
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

        //Movement
        if (snip == false) 
            {
            MoveMode();
            }

        else if (snip == true)
            {
            SnipMode();
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
    }


    // Snip mode
    void SnipMode()
    {
        transform.Translate(Vector3.right * speed * 2 * Time.deltaTime * yInput);
        transform.Translate(Vector3.forward * speed * Time.deltaTime * xInput);
    }

    // Ground Check
    private void OnCollisionEnter(Collision collision)
    {
        onGround = true;
    }

}
