using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Savecrab : MonoBehaviour
{
    public int NumberOfCrabs;
    private int CollectedCrabs = 0;
    public TextMeshProUGUI CrabsText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            CollectedCrabs = CollectedCrabs + 1;
            CrabsText.text = CollectedCrabs + " / " + NumberOfCrabs;
            Destroy(gameObject);

        }
    }
}
