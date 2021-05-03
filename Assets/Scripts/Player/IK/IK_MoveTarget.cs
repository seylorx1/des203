using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_MoveTarget : MonoBehaviour {

    public float
        startHeightOffset = 0.5f,
        rayLength = 1f,
        MaxDistance,
        speed = 5f,
        DistanceSpeedMod = 2.0f;

    public AnimationCurve curve;

    public PlayerController playerController;

    public Transform crabIKInheritTargets;

    private Transform targetTransform;

    private float
        targetDistance,
        targetHitYPoint;

    private int
        layerMask_Player,
        layerMask_OutlineTrigger;

    private bool snapLeg = false;


    void Start() {
        //targetOffset = transform.position - playerController.transform.position;

        targetTransform = new GameObject("FIXED " + transform.name).transform;
        targetTransform.position = transform.position;
        targetHitYPoint = targetTransform.position.y;

        //targetTransform.rotation = transform.rotation;
        targetTransform.parent = crabIKInheritTargets;

        layerMask_Player = LayerMask.GetMask("PlayerCharacter");
        layerMask_OutlineTrigger = LayerMask.GetMask("OutlineTrigger");
    }


    //private void UpdateTargetPosition() {
    //    targetPosition = playerController.transform.position + playerController.transform.TransformDirection(targetOffset);
    //}

    void Update() {

        if (playerController.CrabFlipped) {

            Vector3 circleMovement = new Vector3(
                Mathf.Sin(Time.time * MaxDistance * 50.0f),
                0.0f,
                Mathf.Cos(Time.time * MaxDistance * 50.0f)
                ) * 0.05f;
            
            transform.position =
                targetTransform.position +
                playerController.transform.TransformDirection(circleMovement);
        }
        else {
            if (snapLeg) {
                // Animate legs
                //dist = Vector3.Distance(transform.position, targetPosition);
                Vector3 walkTarget = new Vector3(
                        targetTransform.position.x,
                        targetHitYPoint,
                        targetTransform.position.z);

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    walkTarget,
                    Time.deltaTime * Mathf.Max(speed, targetDistance * DistanceSpeedMod));

                if (transform.position == walkTarget) {
                    snapLeg = false;
                }
            }
        }
    }

    void FixedUpdate() {
        if (!playerController.CrabFlipped) {

            RaycastHit hit;
            if (Physics.Raycast(
                    targetTransform.position + (Vector3.up * startHeightOffset),
                    Vector3.down,
                    out hit,
                    rayLength,
                    ~(layerMask_Player | layerMask_OutlineTrigger) )) {
                targetHitYPoint = hit.point.y;
            }

            // TODO Verify if TODO is still necessary
            // TODO check the rotation of the crab is correct -- don't want to attempt IK if she's flipped!
            targetDistance = Vector3.Distance(transform.position, targetTransform.position);
            if (targetDistance > MaxDistance) {
                snapLeg = true;
            }
        }
    }
}
