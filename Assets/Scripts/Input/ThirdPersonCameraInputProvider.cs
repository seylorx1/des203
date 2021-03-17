using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.AxisState;

public class ThirdPersonCameraInputProvider : MonoBehaviour, IInputAxisProvider {

    private PlayerController playerController;
    private void Awake() {
        playerController = GetComponentInParent<PlayerController>();
    }

    public float GetAxisValue(int axis) {
        if (playerController != null) {
            switch (axis) {
                case 0: //X
                    return playerController.LookAxis.x;
                case 1: //Y
                    return playerController.LookAxis.y;
                case 2: //Z
                    return 0.0f;
            }
        }
        return 0.0f;
    }
}
