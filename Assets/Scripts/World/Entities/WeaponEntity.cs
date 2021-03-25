using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponEntity : WorldEntity {

    private Transform awakeTransform;
    private Rigidbody weaponRigidbody;
    private Collider weaponCollider;

    private ItemPickup claw;
    private bool weaponSwung = false;

    void Awake() {
        awakeTransform = transform.parent;
        weaponRigidbody = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<Collider>();
    }

    void Reset() {
        if (!CompareTag("Grabbable")) {
            tag = "Grabbable";
            Debug.Log($"Set {transform.name}\'s tag to \'Grabbable\'.", gameObject);
        }

        if(GetComponent<Collider>() == null) {
            Debug.LogError($"Please attach a collider to {transform.name}!", gameObject);
        }
    }

    public void Pickup(ItemPickup claw) {
        weaponRigidbody.isKinematic = true;
        weaponRigidbody.useGravity = false;

        weaponCollider.isTrigger = true;

        this.claw = claw;
        transform.parent = claw.transform;
    }

    public void Drop() {
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.useGravity = true;

        weaponCollider.isTrigger = false;

        //Reset the claw.
        claw = null;
        weaponSwung = false;
        transform.parent = awakeTransform;
    }

    public void OnTriggerEnter(Collider other) {
        if (!weaponSwung) {
            if (claw != null && claw.IsClawSwingingItem()) {
                WorldEntity otherEntity = other.GetComponent<WorldEntity>();
                if (otherEntity != null) {

                    otherEntity.Damage(1);
                    Damage(1);

                    weaponSwung = true;
                }
            }
        }
    }

    public void OnTriggerExit(Collider other) {
        weaponSwung = false; // Have to take the weapon away in order to deal damage again!
    }
}
