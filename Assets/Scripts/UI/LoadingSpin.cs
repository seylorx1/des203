using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpin : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        transform.eulerAngles = new Vector3(0.0f, 0.0f, (Time.unscaledTime * -90.0f) % 360.0f);
    }
}
