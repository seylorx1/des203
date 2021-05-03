using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabFloatVolume : MonoBehaviour
{
    public float gravityScale = 10;
    public float moveScale = 1;

    private Rigidbody crabRigidbody;

    private int layer_playerCharacter;
    private int layer_playerCharacterNoCollisions;

    private void Awake() {
        layer_playerCharacter = LayerMask.NameToLayer("PlayerCharacter");
        layer_playerCharacterNoCollisions = LayerMask.NameToLayer("PlayerCharacter (No Collisions)");
    }

    private void FixedUpdate() {
        crabRigidbody?.AddForce(Vector3.up * gravityScale * Time.fixedDeltaTime, ForceMode.Acceleration);
        crabRigidbody?.AddForce(Vector3.left * moveScale * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            other.attachedRigidbody.gameObject.layer = layer_playerCharacterNoCollisions;
            other.attachedRigidbody.useGravity = false;
            crabRigidbody = other.attachedRigidbody;

        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.attachedRigidbody == crabRigidbody) {
                other.attachedRigidbody.gameObject.layer = layer_playerCharacter;
                other.attachedRigidbody.useGravity = true;
                crabRigidbody = null;
            }
        }
    }

}
