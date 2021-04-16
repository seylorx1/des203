using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
    
public class GibInstance : MonoBehaviour {
    private Material materialInstance;
    
    // Start is called before the first frame update
    void Start() {
        materialInstance = GetComponent<MeshRenderer>().material;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Random.rotation * Vector3.up * 2.5f, ForceMode.VelocityChange);
        rb.AddTorque(Random.rotation * Vector3.up, ForceMode.VelocityChange);

        StartCoroutine(FadeDestory());
    }

    IEnumerator FadeDestory() {
        yield return new WaitForSecondsRealtime(5.0f);
        while(materialInstance.color.a > 0.0f) {
            Color c = materialInstance.color;
            c.a -= Time.unscaledDeltaTime;
            materialInstance.color = c;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
        yield return null;
    }
}
