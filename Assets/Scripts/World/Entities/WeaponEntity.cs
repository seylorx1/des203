using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponEntity : WorldEntity {

    private Transform awakeTransform;
    private Rigidbody weaponRigidbody;
    private Collider weaponCollider;

    private bool weaponSwung = false;

    [HideInInspector] public ItemPickup HeldClaw { get; private set; }

    protected override void Awake() {
        base.Awake();

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

        HeldClaw = claw;
        transform.parent = claw.transform;
    }

    public void Drop() {
        weaponRigidbody.isKinematic = false;
        weaponRigidbody.useGravity = true;

        weaponCollider.isTrigger = false;

        //Reset the claw.
        HeldClaw = null;
        weaponSwung = false;
        transform.parent = awakeTransform;
    }


    /// <returns>True, if damage was inflicted.</returns>
    public bool InflictDamage(WorldEntity otherEntity) {
        //check to see if other entity is a weaponentity
        if (otherEntity.GetType() == typeof(WeaponEntity) && ((WeaponEntity)otherEntity).HeldClaw != null) {
            return false; //Don't inflict damage on a held weapon!
        }

        otherEntity.Damage(1);
        Damage(1);

        return true;
    }

    public void OnTriggerEnter(Collider other) {
        if (!weaponSwung) {
            if (HeldClaw != null && HeldClaw.IsClawSwingingItem()) {
                WorldEntity otherEntity = other.GetComponent<WorldEntity>();
                if (otherEntity != null) {
                    weaponSwung = InflictDamage(otherEntity);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other) {
        weaponSwung = false; // Have to take the weapon away in order to deal damage again!
    }
}
