using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {
    public enum Claw { Left, Right };
    public Claw claw;

    public PlayerController playerController;
    public bool isHolding;
    public float step = 4;
    public float swingValue = 0.5f;

    private WeaponEntity heldWeapon = null;

    private float ClawTrigger {
        get {
            if (claw == Claw.Left) {
                return playerController.lTrigger;
            }
            return playerController.rTrigger;
        }
    }

    private Vector2 ClawStick {
        get {
            if (claw == Claw.Left) {
                return playerController.inputLS;
            }
            return playerController.inputRS;
        }
    }

    // Update is called once per frame
    void Update() {
        HandleHeldItems();
    }

    void OnTriggerStay(Collider other) {

        if (!isHolding) {
            if (ClawTrigger > 0.2f) {
                if (other.CompareTag("Grabbable")) {
                    heldWeapon = other.GetComponent<WeaponEntity>();
                    if (heldWeapon != null) {
                        heldWeapon.Pickup(this);
                    }
                    else {
                        other.transform.parent = transform;
                    }
                    isHolding = true;
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
        if (isHolding) {

            if (playerController.Snip) {
                
            }

            if (ClawTrigger < 0.2f) {
                if (heldWeapon != null) {
                    heldWeapon.Drop();
                }
                else {
                    transform.DetachChildren();
                }
                isHolding = false;
            }
        }
    }

    /// <summary>
    /// Detect if the claw is swinging.
    /// </summary>
    /// <returns>True, if an item is held and the absolute value of ClawStick exceeds swingValue.</returns>
    public bool IsClawSwingingItem() {
        return
            isHolding &&
            (Mathf.Abs(ClawStick.x) >= swingValue || Mathf.Abs(ClawStick.y) >= swingValue);
    }
}
