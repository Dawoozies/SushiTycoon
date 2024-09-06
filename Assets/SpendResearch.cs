using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SpendResearch : MonoBehaviour
{
    public int cost;
    public TextMeshProUGUI text;
    public int researchTier;
    public int maxResearchTier;


    private float waiterSpeedIncrease = 0.2f;
    public UnityEvent onSpendResearch;
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        UpdateText();
    }
    void Update()
    {
    }
    public virtual void BuyNext()
    {
        if (RestaurantParameters.ins.ResearchPointsGained >= cost && researchTier < maxResearchTier)
        {
            RestaurantParameters.ins.ResearchPointsGained -= cost;
            researchTier++;
            UpdateText();
            RestaurantParameters.ins.waiterRunningSpeed += waiterSpeedIncrease;
        }
    }

    void UpdateText()
    {
        text.text = researchTier + "/" + maxResearchTier;
    }
}
