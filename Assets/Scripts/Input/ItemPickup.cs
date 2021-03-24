using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public PlayerController playerController;
    public bool isHolding;
    public float step = 4;
    public float swingValue = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HoldItems();

        if (playerController.snip)
        {
            SwingItems();
        }
        
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

            if (other.tag == "Lever" && playerController.rTrigger > 0.2f && playerController.inputRS.x < -0.3 && !isHolding)
            {
                Debug.Log("Lever rotate");
                other.transform.Rotate(step, 0, 0 * Time.deltaTime, Space.Self);
            }

            else if (other.tag == "Lever" && playerController.rTrigger > 0.2f && playerController.inputRS.x > 0.3 && !isHolding)
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

            if (other.tag == "Lever" && playerController.lTrigger > 0.2f && playerController.inputLS.x < -0.3 && !isHolding)
            {
                Debug.Log("Lever rotate");
                other.transform.Rotate(step, 0, 0 * Time.deltaTime, Space.Self);
            }

            else if (other.tag == "Lever" && playerController.lTrigger > 0.2f && playerController.inputLS.x > 0.3 && !isHolding)
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

        if (gameObject.tag == "Left Claw" && playerController.lTrigger < 0.2f && isHolding == true)
        {
            Debug.Log("un-grabbed");
            transform.DetachChildren();
            isHolding = false;
        }
    }

    void SwingItems()
    {
        if (gameObject.tag == "Right Claw" && isHolding)
        {
            if (playerController.inputRS.x >= swingValue || playerController.inputRS.x <= -swingValue || playerController.inputRS.y >= swingValue || playerController.inputRS.y <= -swingValue)
            {
                Debug.Log("Swing!");
            }
        }

        if (gameObject.tag == "Left Claw" && isHolding)
        {
            if (playerController.inputLS.x >= swingValue || playerController.inputLS.x <= -swingValue || playerController.inputRS.y >= swingValue || playerController.inputRS.y <= -swingValue)
            {
                Debug.Log("Swing!");
            }
        }
    }
}
