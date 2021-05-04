using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClawController : MonoBehaviour {
    public enum Claw { Left, Right };
    public Claw claw;

    public ClawController otherClawController;
    public PlayerController playerController;

    public Transform pivotTarget;

    public float
        leverSpeed = 4,
        swingValue = 0.5f,
        pivotSpeed = 20.0f;

    private LeverFunction lastLeverFunction;

    public WeaponEntity HeldWeapon { get; private set; } = null;
    public Transform HeldTransform { get; private set; } = null;

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

        if (lastLeverFunction != null && ClawTrigger > 0.2f && Mathf.Abs(ClawStick.x) > 0.1f ) {

            lastLeverFunction.SetLeverAngle(
                lastLeverFunction.GetLeverAngle() +
                (Mathf.Sign(ClawStick.x) * leverSpeed * Time.deltaTime));
        }
    }

    void OnTriggerStay(Collider other) {

        if (HeldTransform == null) {
            if (ClawTrigger > 0.2f) {
                if (other.CompareTag("Grabbable")) {

                    //If the other held transform is the same as the current other.transform, drop it.
                    if (otherClawController.HeldTransform == other.transform) {
                        otherClawController.HeldWeapon.Drop();
                        otherClawController.HeldWeapon = null;
                        otherClawController.HeldTransform = null;
                    }

                    HeldWeapon = other.GetComponent<WeaponEntity>();
                    if (HeldWeapon != null) {
                        HeldWeapon.Pickup(this);
                    }
                    else {
                        other.transform.parent = transform;
                    }

                    HeldTransform = other.transform;
                    HeldTransform.GetComponent<MeshRenderer>()?.material?.SetFloat("_OutlineHighlight", 0.0f);
                }
            }


            if (other.CompareTag("Lever")) {
                lastLeverFunction = other.GetComponent<LeverFunction>();
            }
            else {
                lastLeverFunction = null;
            }
        }

    }

    void OnTriggerExit(Collider other) {
        lastLeverFunction = null;
    }

    void HandleHeldItems() {
        if (HeldTransform != null) { //Player holding the item.
            if (ClawPivot) {
                HeldTransform.Rotate(
                    playerController.transform.TransformDirection(
                        new Vector3(
                            -ClawStick.y,
                            0.0f,
                            ClawStick.x
                            )),
                    pivotSpeed * Mathf.Clamp01(ClawStick.magnitude) * Time.deltaTime,
                    Space.World);
            }
            if (ClawTrigger < 0.95f) { //Player let go of trigger.
                if (HeldWeapon != null) {
                    HeldWeapon.Drop();
                }
                else {
                    transform.DetachChildren();
                }
                HeldTransform = null;
            }
        }
    }

    /// <summary>
    /// Detect if the claw is swinging.
    /// </summary>
    /// <returns>True, if an item is held and the absolute value of ClawStick exceeds swingValue.</returns>
    public bool IsClawSwingingItem() {
        return
            (HeldTransform != null) && //Has to have item to swing
            playerController.Snip && //Only swing in snip mode
            !ClawPivot && //Don't class as swinging when pivoting
            (Mathf.Abs(ClawStick.x) >= swingValue || Mathf.Abs(ClawStick.y) >= swingValue);
    }
}
