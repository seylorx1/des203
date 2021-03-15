using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_MoveTarget : MonoBehaviour {
    public float rayLength = 1f;

    public float MaxDistance;
    public float speed = 5f;
    public float DistanceSpeedMod = 2.0f;

    public AnimationCurve curve;

    public Transform CrabTransform;

    private Vector3 targetOffset;
    private Vector3 targetPosition;

    private bool snapLeg = false;

    private int layerMask_Player;

    void Start() {
        targetOffset = transform.position - CrabTransform.position;
        targetPosition = transform.position;

        layerMask_Player = LayerMask.GetMask("PlayerCharacter");
    }

    void Update() {
        // Future raycast
        targetPosition = CrabTransform.position + CrabTransform.TransformDirection(targetOffset);

        Debug.DrawRay(targetPosition + Vector3.up, Vector3.down, Color.green);
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
        float targetDistance = Vector3.Distance(transform.position, targetPosition);
        if (targetDistance > MaxDistance) {
            snapLeg = true;
        }

        if(snapLeg) {
            // Animate legs
            //dist = Vector3.Distance(transform.position, targetPosition);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * Mathf.Max(speed, targetDistance * DistanceSpeedMod));

            if(transform.position == targetPosition) {
                snapLeg = false;
            }
        }

    }
}
