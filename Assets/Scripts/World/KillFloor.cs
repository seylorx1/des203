using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KillFloor : MonoBehaviour {
    private Vector3 startPosition;
    private Quaternion startRotation;

    public static float KillFloorHeight = -100.0f;

    public enum Mode {
        Kill,
        Reset
    }
    public Mode killFloorMode = Mode.Reset;

    void Start() {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void FixedUpdate() {
        //Reset position if object falls below a certain y value :)
        if (transform.position.y <= KillFloorHeight) {
            switch (killFloorMode) {
                case Mode.Kill:
                    Destroy(gameObject);
                    break;
                case Mode.Reset:
                    transform.SetPositionAndRotation(startPosition, startRotation);

                    //Should stop shit flying about after getting reset.
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb != null) {
                        rb.velocity = Vector3.zero;
                    }
                    break;
            }
        }
    }
}
