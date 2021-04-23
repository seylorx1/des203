using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelPickupOutline : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            Renderer rend = other.GetComponent<Renderer>();

            rend.material.SetColor("_OutlineColor", Color.yellow);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            Renderer rend = other.GetComponent<Renderer>();

            rend.material.SetColor("_OutlineColor", Color.black);
        }
    }
}
