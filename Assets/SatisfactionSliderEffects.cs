using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SatisfactionSliderEffects : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image handleImage;
    [SerializeField] Gradient handleGradient;
    [SerializeField] GameObject[] satisfactionGraphics;
    Image[] satisfactionImages;
    float currentValue;
    void Start()
    {
        satisfactionImages = new Image[satisfactionGraphics.Length];
        for (int i = 0; i < satisfactionGraphics.Length; i++)
        {
            satisfactionImages[i] = satisfactionGraphics[i].GetComponent<Image>();
            if(i != 0)
            {
                satisfactionGraphics[i].SetActive(false);
            }
        }
    }
    public void OnValueChanged(float value)
    {
        if(currentValue != value)
        {
            float sliderPercentage = value / slider.maxValue;
            int indexActive = Mathf.FloorToInt(Mathf.Lerp(0, satisfactionGraphics.Length, sliderPercentage));
            indexActive = Mathf.Clamp(indexActive, 0, satisfactionGraphics.Length - 1);
            for (int i = 0; i < satisfactionGraphics.Length; i++)
            {
                satisfactionGraphics[i].SetActive(i == indexActive);
                satisfactionImages[i].color = handleGradient.Evaluate(value / slider.maxValue);
            }
            currentValue = value;
        }
    }
}
