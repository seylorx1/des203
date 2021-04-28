using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentagramController : MonoBehaviour {
    Material pentagramMaterial;
    public float pulsationSpeed = 90.0f;
    // Start is called before the first frame update
    void Start() {
        pentagramMaterial = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update() {
        pentagramMaterial.SetFloat("_Cutoff", (Mathf.Sin(Mathf.Deg2Rad * Time.time * pulsationSpeed) + 1.0f) * 0.5f * 0.1f + 0.3f);
    }
}
