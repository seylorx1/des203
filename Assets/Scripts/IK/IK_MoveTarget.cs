using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_MoveTarget : MonoBehaviour
{
    //public float zPos;
    //public float rayLength = 10f;

    public Transform currentTarget;
    public Transform desiredTarget;

    public float dist;

    void Start()
    {
        //zPos = transform.position.z;

    }

    void Update()
    {
        // Future raycast

        /*RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            zPos = hit.point.z;
        }
        else
        {
            zPos = transform.position.z; 
        }*/

        dist = Vector3.Distance(currentTarget.position, desiredTarget.position);
        if (dist > 0.13f)
        {
            currentTarget.position = desiredTarget.position;
        }
    }
}
