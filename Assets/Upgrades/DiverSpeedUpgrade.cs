using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiverSpeedUpgrade : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int researchTier => RestaurantParameters.ins.DiverResearchTier;
    public int maxResearchTier => RestaurantParameters.ins.DiverMaxResearchTier;
    public float diverRunningSpeed => RestaurantParameters.ins.DiverRunningSpeed;
    public float diverSpeedIncrease => RestaurantParameters.ins.DiverSpeedIncrease;
    public int cost => RestaurantParameters.ins.DiverResearchCost;
    public bool upgradable => researchTier < maxResearchTier;


    void Start()
    {
        SpendResearch spendResearch = GetComponent<SpendResearch>();
        spendResearch.AddListener(OnUpgrade);
        text = GetComponentInChildren<TextMeshProUGUI>();
        UpdateText();
    }
    public void OnUpgrade()
    {
        if (cost < RestaurantParameters.ins.ResearchPointsGained && upgradable)
        {
            RestaurantParameters.ins.ResearchPointsGained -= cost;
            RestaurantParameters.ins.DiverRunningSpeed += diverSpeedIncrease;
            RestaurantParameters.ins.DiverResearchTier++;
            UpdateText();
        }
    }
    void UpdateText()
    {
        text.text = researchTier + "/" + maxResearchTier + "\nCurrent: " + Mathf.Round(diverRunningSpeed * 100f) / 100f;
        if (upgradable)
        {
            text.text = text.text + "\nNext: " + Mathf.Round((diverRunningSpeed + diverSpeedIncrease) * 100f) / 100f;
        }
    }
}
