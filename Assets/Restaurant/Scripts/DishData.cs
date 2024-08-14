using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class DishData : ScriptableObject
{
    public float priceBase;
    //recipe
    //stage of prep 0 - collectable0,amount0
    //stage of prep 1 - collectable0,amount1
    [ReorderableList]
    public List<PreperationStage> preparation;
    [ReorderableList]
    public List<AddonsStage> addons;
}
[Serializable]
public class PreperationStage
{
    public string prepStageDescription;
    public CollectableData ingredient;
    public int ingredientAmount;
    public float stageTime;
}

[Serializable]
public class AddonsStage
{
    public string addonsStageDescription;
    public AddonData addon;
    public int addonAmount;
    public float stageTime;
}