using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientStorage : MonoBehaviour
{
    public static IngredientStorage ins;
    private void Awake()
    {
        ins = this;
    }
    [SerializeField] SerializedDictionary<CollectableData, int> ingredients = new();
    public void AddToStorage(CollectableData collectable, int amount)
    {
        if(!ingredients.TryAdd(collectable, amount))
        {
            ingredients[collectable] += amount;
        }

        RestaurantParameters.ins.UpdateMenu();
    }
    public bool TryGetIngredients(CollectableData ingredient, int requestAmount, out int amount)
    {
        amount = 0;
        if (!ingredients.ContainsKey(ingredient))
            return false;
        if (ingredients[ingredient] < requestAmount)
            return false;

        amount = requestAmount;
        ingredients[ingredient] -= requestAmount;

        return true;
    }
    public bool TryGetIngredientMultiple(CollectableData ingredient, int requiredAmount, out int multiple)
    {
        multiple = 0;
        if (!ingredients.ContainsKey(ingredient))
            return false;
        if (ingredients[ingredient] < requiredAmount)
            return false;
        if(requiredAmount == 0)
        {
            //if we dont need any ingredient amount to make this then
            //it is infinite source
            //making this 1 for now
            multiple = 999;
            return true;
        }

        int amountInStorage = ingredients[ingredient];
        while(amountInStorage > 0)
        {
            amountInStorage -= requiredAmount;
            if(amountInStorage >= 0)
            {
                multiple++;
            }
        }
        return true;
    }
}
