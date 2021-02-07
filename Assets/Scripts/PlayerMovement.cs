using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables 
    private Rigidbody rigbod;
    private float xInput;
    private float yInput;
    public float jumpForce = 4;
    public bool snip;
    public bool onGround;


    // Start is called before the first frame update
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
            transform.Translate(Vector3.right * 10 * Time.deltaTime * xInput);
            transform.Translate(Vector3.forward * 5 * Time.deltaTime * yInput);
            }

        else if (snip == true)
            {
            transform.Translate(Vector3.right * 10 * Time.deltaTime * yInput);
            transform.Translate(Vector3.forward * 5 * Time.deltaTime * xInput);
            }
        
        if (Input.GetKeyDown(KeyCode.Joystick1Button0) && onGround == true)
        {
            rigbod.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            onGround = false;
        }
    }

    // Ground Check
    private void OnCollisionEnter(Collision collision)
    {
        onGround = true;
    }

}
