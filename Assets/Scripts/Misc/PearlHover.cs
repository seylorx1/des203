using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PearlHover : MonoBehaviour {

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start() {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        transform.position = startPosition + new Vector3(0.0f, 0.05f * Mathf.Sin(Time.time * 2.0f), 0.0f);
    }
}
