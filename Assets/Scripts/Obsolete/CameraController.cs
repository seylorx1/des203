using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 1;
    public Transform target, player;
    float rsX, rsY; /* rs = right stick */

    public Transform obstruction;
    public float zoomSpeed = 2f;
    public float zoomSensitivity = 3f;
    public float maxDistance = 4.5f;
    public float minDistance = 1.5f;

    void Start()
    {
        obstruction = target;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CamControl();
        ViewObstructed();
    }

    void CamControl()
    {
        rsX += Input.GetAxis("RStick Horizontal") * rotationSpeed;
        rsY -= Input.GetAxis("RStick Vertical") * rotationSpeed;
        rsY = Mathf.Clamp(rsY, -35, 60);

        transform.LookAt(target);

        target.rotation = Quaternion.Euler(rsY, rsX, 0);
    }

    void ViewObstructed()
    {
     //obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, maxDistance))
        {
            if(hit.collider.gameObject.tag == "Environment")
            {
                obstruction = hit.transform;
                //obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                if(Vector3.Distance(obstruction.position, transform.position) >= zoomSensitivity && Vector3.Distance(transform.position, target.position) >= minDistance)
                {
                    transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);
                }
            }

            else
            {
                //obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                if(Vector3.Distance(transform.position, target.position) < maxDistance)
                {
                    transform.Translate(Vector3.back * zoomSpeed * Time.deltaTime);        
                }
            }
        }
    }

}
