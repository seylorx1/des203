using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabButton : MonoBehaviour, IInteractable
{
    public CrabButtonActor actor;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            actor.Pressed(other);
        }
    }

    public IInteractable.Type GetInteractableType() {
        return IInteractable.Type.Interactable;
    }
}
