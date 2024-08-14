using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class DishData : ScriptableObject
{
    //recipe
    //stage of prep 0 - collectable0,amount0
    //stage of prep 1 - collectable0,amount1
    [ReorderableList]
    public List<PreperationStage> preparation;
}
[Serializable]
public class PreperationStage
{
    public string prepStageDescription;
    public CollectableData ingredient;
    public int ingredientAmount;
    public float stageTime;
}