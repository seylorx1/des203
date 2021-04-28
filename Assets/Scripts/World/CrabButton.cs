using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabButton : MonoBehaviour
{
    public CrabButtonActor actor;
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            actor.Pressed(other);
        }
    }
}
