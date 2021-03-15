using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timertext;
    private float seconds = 0f;
    private string secondsnodecimal;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        seconds += Time.deltaTime;
        secondsnodecimal = seconds.ToString("F0");
        timertext.text = "" + secondsnodecimal;
    }
}
