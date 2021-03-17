using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public PlayerController playerController;
    public bool isHolding;

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
        if (gameObject.tag == "Right Claw" && other.tag == "Grabbable" && playerController.rTrigger > 0.2f)
        {
            Debug.Log("grabbed");
            other.transform.parent = gameObject.transform;
            isHolding = true;
        }

        else if (gameObject.tag == "Left Claw" && other.tag == "Grabbable" && playerController.lTrigger > 0.2f)
        {
            Debug.Log("grabbed");
            other.transform.parent = gameObject.transform;
            isHolding = true;
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
