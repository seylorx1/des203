using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    public float gravityScale = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player") || !other.gameObject.CompareTag("NoFloat"))
        {
            other.attachedRigidbody.useGravity = false;
            other.attachedRigidbody.AddForce(transform.up * -gravityScale);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            other.attachedRigidbody.useGravity = true;
        }
    }
}
