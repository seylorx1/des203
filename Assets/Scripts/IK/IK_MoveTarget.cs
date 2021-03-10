using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_MoveTarget : MonoBehaviour
{
    public float yPos;
    
    public float rayLength = 1f;

    
    public Transform currentTarget; /* The target the leg snaps to */
    public Transform desiredTarget; /* Where the leg snap target should move towards */

    public float dist;
    public float timer;
    public float speed = 5f;

    public AnimationCurve curve;

    void Start()
    {
        yPos = transform.position.y;

    }

    void Update()
    {
        // Future raycast

        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down, Color.green);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            Debug.Log("hit");
            yPos = hit.point.y;
            desiredTarget.position = new Vector3(transform.position.x, yPos, transform.position.z);

        }

        
        
        /*else
        {
            yPos = transform.position.y; 
        }*/


        // Animate legs
        dist = Vector3.Distance(currentTarget.position, desiredTarget.position);

        if (dist > 0.13f)
        {
            timer = 0;
            timer += Time.deltaTime;

            currentTarget.position = Vector3.MoveTowards(desiredTarget.position, new Vector3(desiredTarget.position.x, desiredTarget.position.y /*+ curve.Evaluate(timer)*/, desiredTarget.position.z), speed * Time.deltaTime);
        }
        
    }
}
