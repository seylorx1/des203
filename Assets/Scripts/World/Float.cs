using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    public float gravityScale = 10;
    public float moveScale = 1;

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
            other.attachedRigidbody.AddForce(transform.up * gravityScale);
            other.attachedRigidbody.AddForce(-transform.right * moveScale);
        }
    }

    private void OnTriggerStay(Collider other)
    {
      
        if (!other.gameObject.CompareTag("Player") || !other.gameObject.CompareTag("NoFloat"))
        {
            other.attachedRigidbody.AddForce(transform.up * gravityScale * Time.deltaTime);
            other.attachedRigidbody.AddForce(-transform.right * moveScale * Time.deltaTime);
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
