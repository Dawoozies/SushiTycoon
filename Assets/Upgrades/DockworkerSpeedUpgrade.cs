using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DockworkerSpeedUpgrade : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int researchTier => RestaurantParameters.ins.DockWorkerResearchTier;
    public int maxResearchTier => RestaurantParameters.ins.DockWorkerMaxResearchTier;
    public float dockWorkerRunningSpeed => RestaurantParameters.ins.DockWorkerRunningSpeed;
    public float dockWorkerSpeedIncrease => RestaurantParameters.ins.DockWorkerSpeedIncrease;
    public int cost => RestaurantParameters.ins.DockWorkerResearchCost;
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
            RestaurantParameters.ins.DockWorkerRunningSpeed += dockWorkerSpeedIncrease;
            RestaurantParameters.ins.DockWorkerResearchTier++;
            UpdateText();
        }
    }
    void UpdateText()
    {
        text.text = researchTier + "/" + maxResearchTier + "\nCurrent: " + Mathf.Round(dockWorkerRunningSpeed * 100f) / 100f;
        if (upgradable)
        {
            text.text = text.text + "\nNext: " + Mathf.Round((dockWorkerRunningSpeed + dockWorkerSpeedIncrease) * 100f) / 100f;
        }
    }
}
