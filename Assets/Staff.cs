using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;
using BayatGames.SaveGameFree;
using TMPro;
public class Staff : MonoBehaviour
{
    public string saveFileName;
    public GameObject prefab;
    public Transform spawnPoint;
    public float hiringBaseCost;
    public AnimationCurve hiringCurve;
    [SerializeField] int numberOfStaff;
    [SerializeField] string staffName;
    [SerializeField] TMP_Text hiringButtonText;
    float hiringCurveParameter => (float) numberOfStaff / (float) RestaurantParameters.ins.StaffUpperBound;
    int hiringCost => Mathf.RoundToInt(hiringBaseCost * hiringCurve.Evaluate(hiringCurveParameter));
    private void Start()
    {
        Load();
        for (int i = 0; i < numberOfStaff; i++)
        {
            GameObject poolObj = SharedGameObjectPool.Rent(prefab);
            poolObj.transform.position = spawnPoint.position;
        }
    }
    private void Update()
    {
        hiringButtonText.text = $"Hire: {staffName} \n ${hiringCost}";
    }
    public void Hire()
    {
        if(RestaurantParameters.ins.TryBuyItem(hiringCost))
        {
            GameObject poolObj = SharedGameObjectPool.Rent(prefab);
            poolObj.transform.position = spawnPoint.position;
            numberOfStaff++;
            Save();
        }
    }
    void Load()
    {
        if(SaveGame.Exists(saveFileName))
            numberOfStaff = SaveGame.Load<int>(saveFileName);
    }
    void Save()
    {
        SaveGame.Save(saveFileName, numberOfStaff);
    }
}