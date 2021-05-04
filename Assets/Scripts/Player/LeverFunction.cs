using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverFunction : MonoBehaviour, IInteractable {
    public float angleCentre = 90.0f;
    public float angleStart = 80.0f;

    public float gateSpeed = 1f;
    public float range = 30.0f;

    public Transform gate, target;

    private float angleRotation = 0.0f;
    private bool openGate = false;

    // Update is called once per frame
    void Start() {
        SetLeverAngle(angleStart);
    }

    void Update() {
        if (angleRotation > 100.0f) {
            openGate = true;
        }

        if(openGate) {
            gate.position = Vector3.Lerp(gate.position, target.position, gateSpeed * Time.deltaTime);
        }
    }

    public float GetLeverAngle() {
        return angleRotation;
    }

    public void SetLeverAngle(float angle) {

        angleRotation = Mathf.Clamp(angle, angleCentre - range, angleCentre + range);
        transform.localRotation = Quaternion.AngleAxis(angleRotation, Vector3.left);
    }

    public IInteractable.Type GetInteractableType() {
        return IInteractable.Type.Interactable;
    }
}
