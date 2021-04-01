using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverFunction : MonoBehaviour
{
    public float rotation;
    public float speed = 1f;
    public bool openGate;
    public GameObject gate;
    public Transform target;
    public ObjectiveHandler objectiveHandler;

    // Update is called once per frame
    void Update()
    {
        rotation = transform.rotation.x;

        if (rotation >= -0.3f ) 
        {
            //Debug.Log("open");
            openGate = true;            
        }

        if (openGate == true)
        {
            gate.transform.position = Vector3.Lerp(gate.transform.position, target.transform.position, speed * Time.deltaTime);
            objectiveHandler.openGate = true;
        }
    }

}
