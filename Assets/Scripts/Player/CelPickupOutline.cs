using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelPickupOutline : MonoBehaviour {
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Grabbable")) {
            Renderer rend = other.GetComponent<Renderer>();
            rend.material.SetFloat("_OutlineHighlight", 1.0f);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Grabbable")) {
            Renderer rend = other.GetComponent<Renderer>();
            rend.material.SetFloat("_OutlineHighlight", 0.0f);
        }
    }
}
