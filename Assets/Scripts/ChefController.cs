using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefController : CrabButtonActor
{
    public GameObject standard, dance;
    public Transform teleportTo;
    public MusicSource musicSource;

    private Transform lastColliderTransform = null;
    private bool teleported = false;    
    public void Awake() {
        standard.SetActive(true);
        dance.SetActive(false);
        musicSource.gameObject.SetActive(false);
    }

    public override void Pressed(Collider other) {
        if (!teleported) {
            standard.SetActive(false);
            dance.SetActive(true);

            lastColliderTransform = other.transform;

            StartCoroutine(Teleport());
            teleported = true;
        }
    }

    IEnumerator Teleport() {
        yield return new WaitForSeconds(0.25f);
        lastColliderTransform.SetPositionAndRotation(teleportTo.position, teleportTo.rotation);
        teleported = false;
        musicSource.gameObject.SetActive(true);
    }
}
