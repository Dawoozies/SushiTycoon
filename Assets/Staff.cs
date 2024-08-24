using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class Staff : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawnPoint;
    public float hiringCost;
    public AnimationCurve hiringCurve;
    public void Hire()
    {
        if(RestaurantParameters.ins.TryBuyItem(hiringCost))
        {
            GameObject poolObj = SharedGameObjectPool.Rent(prefab);
            poolObj.transform.position = spawnPoint.position;
        }
    }
}