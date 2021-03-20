using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {
    private TextMeshProUGUI timerText;
    private float seconds = 0f;


    // Start is called before the first frame update
    void Start() {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        seconds += Time.deltaTime;

        timerText.text =
            "Time: " +
            Mathf.FloorToInt(seconds / 60.0f).ToString("D2") +
            ":" +
            Mathf.FloorToInt(seconds % 60.0f).ToString("D2");
    }
}
