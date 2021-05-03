using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Savecrab : MonoBehaviour
{
    public int NumberOfCrabs;
    public int CollectedCrabs = 0;
    public TextMeshProUGUI CrabsText;
    public string gibID = "";
    private MeshFilter meshFilter;

    protected GibManagerSingleton.GibBase gib;
    // Start is called before the first frame update
    void Start()
    {
        gib = SingletonManager.Instance.GetSingleton<GibManagerSingleton>().GetGib(gibID);
    }

    protected virtual void Awake()
    {
        if (gibID == "")
        {
            gibID = "metal"; //Default to the metal gib.
        }

        meshFilter = GetComponentInChildren<MeshFilter>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Exit")
        {
            gib?.SpawnParticlesAtPosition(transform.position);
            CollectedCrabs = CollectedCrabs + 1;
            CrabsText.text = CollectedCrabs + " / " + NumberOfCrabs;
            gameObject.SetActive(false);

        }
    }
}
