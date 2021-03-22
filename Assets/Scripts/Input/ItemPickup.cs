using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public PlayerController playerController;
    public bool isHolding;
    public float step = 4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HoldItems();

    }
    void OnTriggerStay(Collider other)
    {
        if (gameObject.tag == "Right Claw")  
        {
            if (other.tag == "Grabbable" && playerController.rTrigger > 0.2f)
            {
                Debug.Log("grabbed");
                other.transform.parent = gameObject.transform;
                isHolding = true;
            }

            if (other.tag == "Lever" && playerController.rTrigger > 0.2f && playerController.inputRS.x < -0.3)
            {
                Debug.Log("Lever rotate");
                other.transform.Rotate(step, 0, 0 * Time.deltaTime, Space.Self);
            }

            else if (other.tag == "Lever" && playerController.rTrigger > 0.2f && playerController.inputRS.x > 0.3)
            {
                Debug.Log("Lever rotate");
                other.transform.Rotate(-step, 0, 0 * Time.deltaTime, Space.Self);
            }
        }

        else if (gameObject.tag == "Left Claw")
        {
            if (other.tag == "Grabbable" && playerController.lTrigger > 0.2f)
            {
                Debug.Log("grabbed");
                other.transform.parent = gameObject.transform;
                isHolding = true;
            }

            if (other.tag == "Lever" && playerController.lTrigger > 0.2f && playerController.inputLS.x < -0.3)
            {
                Debug.Log("Lever rotate");
                other.transform.Rotate(step, 0, 0 * Time.deltaTime, Space.Self);
            }

            else if (other.tag == "Lever" && playerController.lTrigger > 0.2f && playerController.inputLS.x > 0.3)
            {
                Debug.Log("Lever rotate");
                other.transform.Rotate(-step, 0, 0 * Time.deltaTime, Space.Self);
            }
        }
    }

    void HoldItems()
    {
        if (gameObject.tag == "Right Claw" && playerController.rTrigger < 0.2f && isHolding == true)
        {
            Debug.Log("un-grabbed");
            transform.DetachChildren();
            isHolding = false;
        }

        else if (gameObject.tag == "Left Claw" && playerController.lTrigger < 0.2f && isHolding == true)
        {
            Debug.Log("un-grabbed");
            transform.DetachChildren();
            isHolding = false;
        }
    }
}
