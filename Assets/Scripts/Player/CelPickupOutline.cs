using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelPickupOutline : MonoBehaviour {
    // Start is called before the first frame update
    public ClawController
        leftClaw,
        rightClaw;

    private bool TransformHeld(Transform otherTransform) {
        return leftClaw.HeldTransform == otherTransform || rightClaw.HeldTransform == otherTransform;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Grabbable") && !TransformHeld(other.transform)) {
            Renderer rend = other.GetComponent<Renderer>();
            rend.material.SetFloat("_OutlineHighlight", 1.0f);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Grabbable") && !TransformHeld(other.transform)) {
            Renderer rend = other.GetComponent<Renderer>();
            rend.material.SetFloat("_OutlineHighlight", 0.0f);
        }
    }
}
