using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaiterSpeedUpgrade : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int researchTier => RestaurantParameters.ins.WaiterResearchTier;
    public int maxResearchTier => RestaurantParameters.ins.WaiterMaxResearchTier;
    public float waiterRunningSpeed => RestaurantParameters.ins.WaiterRunningSpeed;
    public float waiterSpeedIncrease => RestaurantParameters.ins.WaiterSpeedIncrease;
    public int cost => RestaurantParameters.ins.WaiterResearchCost;
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
            RestaurantParameters.ins.WaiterRunningSpeed += waiterSpeedIncrease;
            RestaurantParameters.ins.WaiterResearchTier++;
            UpdateText();
        }
    }
    void UpdateText()
    {
        text.text = researchTier + "/" + maxResearchTier + "\nCurrent: " + Mathf.Round(waiterRunningSpeed*100f)/100f;
        if (upgradable)
        {
            text.text = text.text + "\nNext: " + Mathf.Round((waiterRunningSpeed + waiterSpeedIncrease)*100f)/100f;
        }
    }
}
