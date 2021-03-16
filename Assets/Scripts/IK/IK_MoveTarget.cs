using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_MoveTarget : MonoBehaviour {

    public float
        rayLength = 1f,
        MaxDistance,
        speed = 5f,
        DistanceSpeedMod = 2.0f;

    public AnimationCurve curve;

    public PlayerController playerController;

    private Vector3
        targetOffset,
        targetPosition;

    private float targetDistance;

    private int layerMask_Player;

    private bool snapLeg = false;


    void Start() {
        targetOffset = transform.position - playerController.transform.position;
        targetPosition = transform.position;

        layerMask_Player = LayerMask.GetMask("PlayerCharacter");
    }


    private void UpdateTargetPosition() {
        targetPosition = playerController.transform.position + playerController.transform.TransformDirection(targetOffset);
    }

    void Update() {

        if (playerController.GetCrabFlipped) {
            UpdateTargetPosition();

            Vector3 circleMovement = new Vector3(
                Mathf.Sin(Time.time * MaxDistance * 50.0f),
                0.0f,
                Mathf.Cos(Time.time * MaxDistance * 50.0f)
                ) * 0.05f;
            targetPosition += playerController.transform.TransformDirection(circleMovement);
            transform.position = targetPosition;
        }
        else {
            if (snapLeg) {
                // Animate legs
                //dist = Vector3.Distance(transform.position, targetPosition);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * Mathf.Max(speed, targetDistance * DistanceSpeedMod));

                if (transform.position == targetPosition) {
                    snapLeg = false;
                }
            }
        }
    }

    void FixedUpdate() {
        if (!playerController.GetCrabFlipped) {
            UpdateTargetPosition();

            RaycastHit hit;
            if (Physics.Raycast(
                    targetPosition + Vector3.up,
                    Vector3.down,
                    out hit,
                    rayLength,
                    ~layerMask_Player)) {
                targetPosition.y = hit.point.y;
            }

            // TODO check the rotation of the crab is correct -- don't want to attempt IK if she's flipped!
            targetDistance = Vector3.Distance(transform.position, targetPosition);
            if (targetDistance > MaxDistance) {
                snapLeg = true;
            }
        }
    }
}
