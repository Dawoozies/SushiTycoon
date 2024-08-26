using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class DishData : ScriptableObject
{
    public Sprite sprite;
    public float priceBase;
    public float eatingTime;
    //recipe
    //stage of prep 0 - collectable0,amount0
    //stage of prep 1 - collectable0,amount1
    [ReorderableList]
    public List<PreperationStage> preparation;
    [ReorderableList]
    public List<AddonsStage> addons;
    public int AmountAvailable()
    {
        int lowestMultiple = int.MaxValue;
        foreach (PreperationStage prepStage in preparation)
        {
            CollectableData ingredient = prepStage.ingredient;
            int ingredientAmount = prepStage.ingredientAmount;
            //as soon as we get a multiple result of 0 then we can break
            int ingredientMultiple = 0;
            if(RestaurantParameters.ins.TryGetIngredientMultiple(ingredient, ingredientAmount, out ingredientMultiple))
            {
                if(ingredientMultiple < lowestMultiple)
                {
                    lowestMultiple = ingredientMultiple;
                }
            }

            if (ingredientMultiple == 0)
            {
                lowestMultiple = 0;
                break;
            }
        }

        return lowestMultiple;
    }
    public void UseUpIngredients()
    {
        foreach (PreperationStage prepStage in preparation)
        {
            int amountUsed = 0;
            RestaurantParameters.ins.TryGetIngredients(prepStage.ingredient, prepStage.ingredientAmount, out amountUsed);
        }
    }
}
[Serializable]
public class PreperationStage
{
    public string prepStageDescription;
    public CollectableData ingredient;
    public int ingredientAmount;
    public float stageTime;
    public KitchenObject.ObjectID requiredPrepStation;
}

[Serializable]
public class AddonsStage
{
    public string addonsStageDescription;
    public AddonData addon;
    public int addonAmount;
    public float stageTime;
}