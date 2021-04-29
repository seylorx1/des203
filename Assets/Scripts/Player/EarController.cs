using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarController : MonoBehaviour {
    public float yawOffset = 180.0f;

    public void Update() {
        transform.rotation = Quaternion.Euler(0.0f, Camera.main.transform.localEulerAngles.y + yawOffset, 0.0f);
    }
}
