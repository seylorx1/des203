using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelInteractionOutline : MonoBehaviour {
    // Start is called before the first frame update
    public ClawController
        leftClaw,
        rightClaw;

    private bool TransformHeld(Transform otherTransform) {
        return leftClaw.HeldTransform == otherTransform || rightClaw.HeldTransform == otherTransform;
    }

    private void OnTriggerEnter(Collider other) {
        if (!TransformHeld(other.transform)) {

            IInteractable.Type interactableType = GetInteractableType(other);
            if (interactableType == IInteractable.Type.Uninteractable) {
                return;
            }

            Renderer[] rends = other.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends) {
                switch(interactableType) {
                    case IInteractable.Type.Interactable:
                        rend.material.SetFloat("_OutlineInteractableHighlight", 1.0f);
                        break;
                    case IInteractable.Type.Breakable:
                        rend.material.SetFloat("_OutlineBreakableHighlight", 1.0f);
                        break;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!TransformHeld(other.transform)) {
            IInteractable.Type interactableType = GetInteractableType(other);
            if(interactableType == IInteractable.Type.Uninteractable) {
                return;
            }

            Renderer[] rends = other.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends) {
                rend.material.SetFloat("_OutlineInteractableHighlight", 0.0f);
                rend.material.SetFloat("_OutlineBreakableHighlight", 0.0f);
            }
        }
    }

    private static IInteractable.Type GetInteractableType(Collider other) {
        
        //Check to see if the collider (or its children) has a component interfacing IInteractable.
        IInteractable interactable = other.gameObject.GetComponentInChildren<IInteractable>();
        if(interactable != null) {
            return interactable.GetInteractableType();
        }

        //Failing that, check the collider's tag.
        switch (other.tag) {
            case "Button":
                return IInteractable.Type.Interactable;
            default:
                return IInteractable.Type.Uninteractable;
        }   
    }
}
