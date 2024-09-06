using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChefSpeedUpgrade : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int researchTier => RestaurantParameters.ins.ChefResearchTier;
    public int maxResearchTier => RestaurantParameters.ins.ChefMaxResearchTier;
    public float chefRunningSpeed => RestaurantParameters.ins.ChefRunningSpeed;
    public float chefSpeedIncrease => RestaurantParameters.ins.ChefSpeedIncrease;
    public int cost => RestaurantParameters.ins.ChefResearchCost;
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
            RestaurantParameters.ins.ChefRunningSpeed += chefSpeedIncrease;
            RestaurantParameters.ins.ChefResearchTier++;
            UpdateText();
        }
    }
    void UpdateText()
    {
        text.text = researchTier + "/" + maxResearchTier + "\nCurrent: " + Mathf.Round(chefRunningSpeed * 100f) / 100f;
        if (upgradable)
        {
            text.text = text.text + "\nNext: " + Mathf.Round((chefRunningSpeed + chefSpeedIncrease) * 100f) / 100f;
        }
    }
}
