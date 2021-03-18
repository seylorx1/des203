using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatSlider : MonoBehaviour
{
    public PlayerController playerController;
    private Slider slider;


    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>(); 
    }

    // Update is called once per frame
    void Update()
    {
        float fillValue = playerController.heat;
        slider.value = fillValue;
    }
}
