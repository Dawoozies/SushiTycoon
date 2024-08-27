using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SatisfactionDisplay : MonoBehaviour
{
    public Slider slider;
    void Update()
    {
        slider.maxValue = RestaurantParameters.ins.SatisfactionPerSecondThreshold;
        slider.value = RestaurantParameters.ins.SatisfactionPerSecond;
    }
}
