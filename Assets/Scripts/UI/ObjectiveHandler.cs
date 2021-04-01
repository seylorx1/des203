using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveHandler : MonoBehaviour
{
    public bool 
        // Objectives
        escapeFishTank,
        openGate,
        escapeRestaurant,
        openChute,

        // Bonuses
        gotCaptured,
        allPearlsCollected,
        destroyedAll,
        destroyedNone;

    // UI Toggle Components
    public Toggle
        openGateToggle,
        escapeFishTankToggle,
        openChuteToggle,
        escapeRestaurantToggle;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (openGate == true)
        {
            openGateToggle.isOn = true;
        }   

        if (escapeFishTank == true)
        {
            escapeFishTankToggle.isOn = true;
        }

        if (openChute == true)
        {
            openChuteToggle.isOn = true;
        }

        if (escapeRestaurant == true)
        {
            escapeRestaurantToggle.isOn = true;
        }
    }


}
