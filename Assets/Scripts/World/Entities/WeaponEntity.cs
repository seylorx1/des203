using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponEntity : WorldEntity {

    public bool spin = false;

    private Transform awakeTransform;
    private Rigidbody weaponRigidbody;
    private Collider weaponCollider;

    private Vector3 spinPosition;

    private bool weaponSwung = false;

    [HideInInspector] public ClawController HeldClaw { get; private set; }

    protected override void Awake() {
        base.Awake();

        awakeTransform = transform.parent;
        weaponRigidbody = GetComponent<Rigidbody>();
        weaponCollider = GetComponent<Collider>();

        spinPosition = transform.position;
    }

    private void Update() {
        if(spin) {
            weaponRigidbody.isKinematic = true;
            weaponRigidbody.useGravity = false;

            weaponCollider.isTrigger = true;

            transform.Rotate(Vector3.up, Time.deltaTime * 360.0f * 0.5f, Space.World);
            transform.position = spinPosition + new Vector3(0.0f, Mathf.Sin(Time.time) * 0.05f, 0.0f);
        }
        else {
            spinPosition = transform.position;
        }
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

    public void Pickup(ClawController claw) {
        spin = false;

        weaponRigidbody.isKinematic = true;
        weaponRigidbody.useGravity = false;

        weaponCollider.isTrigger = true;

        HeldClaw = claw;
        transform.parent = claw.transform;
    }

    public void Drop() {
        spin = false;

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

    public override IInteractable.Type GetInteractableType() {
        return IInteractable.Type.Interactable;
    }
}
