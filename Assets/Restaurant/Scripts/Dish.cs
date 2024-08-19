using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dish : MonoBehaviour
{
    [SerializeField, Disable] Chef assignedChef;
    [SerializeField, Disable] DishData dishData;
    [SerializeField, Disable] List<PreperationStage> unfinishedPrep = new();
    float stageTime;
    float eatingTime;
    public void InitializeDish(Chef initializingChef, DishData dishData)
    {
        assignedChef = initializingChef;
        this.dishData = dishData;
        unfinishedPrep.Clear();
        foreach (PreperationStage prepStage in dishData.preparation)
        {
            unfinishedPrep.Add(prepStage);
        }
    }
    public bool TryPrepDish()
    {
        if(unfinishedPrep.Count > 0)
        {
            if(stageTime < unfinishedPrep[0].stageTime)
            {
                stageTime += Time.deltaTime;
            }
            else
            {
                unfinishedPrep.RemoveAt(0);
                stageTime = 0f;
            }
            return false;
        }
        return true;
    }
    public string DishNameText()
    {
        return dishData.name;
    }
    public string PrepDescriptionText()
    {
        if (unfinishedPrep.Count == 0)
            return "Dish Complete!";
        return unfinishedPrep[0].prepStageDescription;
    }
    public bool TryEatingDish()
    {
        if(eatingTime < dishData.eatingTime)
        {
            eatingTime += Time.deltaTime;
            return false;
        }

        return true;
    }
}