using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour {
    public enum Claw { Left, Right };
    public Claw claw;

    public PlayerController playerController;
    public Transform pivotTarget;

    public float
        step = 4,
        swingValue = 0.5f,
        pivotSpeed = 20.0f;

    private WeaponEntity heldWeapon = null;
    private Transform heldTransform = null;

    private float ClawTrigger {
        get {
            return claw == Claw.Left ? playerController.lTrigger : playerController.rTrigger;
        }
    }

    private Vector2 ClawStick {
        get {
            return claw == Claw.Left ? playerController.inputLS : playerController.inputRS;
        }
    }

    private bool ClawPivot {
        get {
            return claw == Claw.Left ? playerController.LeftClawPivot : playerController.RightClawPivot;
        }
    }

    void Update() {
        HandleHeldItems();
    }

    void OnTriggerStay(Collider other) {

        if (heldTransform == null) {
            if (ClawTrigger > 0.2f) {
                if (other.CompareTag("Grabbable")) {
                    heldWeapon = other.GetComponent<WeaponEntity>();
                    if (heldWeapon != null) {
                        heldWeapon.Pickup(this);
                    }
                    else {
                        other.transform.parent = transform;
                    }

                    heldTransform = other.transform;
                }

                if (other.CompareTag("Lever")) {
                    if (ClawStick.x < -0.3f) {
                        other.transform.Rotate(step * Time.deltaTime, 0, 0 , Space.Self);
                    }
                    else if (ClawStick.x > 0.3f) {
                        other.transform.Rotate(-step * Time.deltaTime, 0, 0, Space.Self);
                    }
                }
            }
        }
    }

    void HandleHeldItems() {
        if (heldTransform != null) { //Player holding the item.
            if (ClawPivot) {
                heldTransform.RotateAround(
                    pivotTarget.position,
                    new Vector3(
                        ClawStick.y,
                        0.0f,
                        ClawStick.x
                        ),
                    pivotSpeed * Mathf.Clamp01(ClawStick.magnitude) * Time.deltaTime);
            }
            if (ClawTrigger < 0.95f) { //Player let go of trigger.
                if (heldWeapon != null) {
                    heldWeapon.Drop();
                }
                else {
                    transform.DetachChildren();
                }
                heldTransform = null;
            }
        }
    }

    /// <summary>
    /// Detect if the claw is swinging.
    /// </summary>
    /// <returns>True, if an item is held and the absolute value of ClawStick exceeds swingValue.</returns>
    public bool IsClawSwingingItem() {
        return
            (heldTransform != null) && //Has to have item to swing
            playerController.Snip && //Only swing in snip mode
            !ClawPivot && //Don't class as swinging when pivoting
            (Mathf.Abs(ClawStick.x) >= swingValue || Mathf.Abs(ClawStick.y) >= swingValue);
    }
}
