using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuyIngredient : MonoBehaviour
{
    public TMP_Text text;
    public CollectableData toBuy;
    public float itemCostBase;
    public int quantity;
    private void Update()
    {
        text.text = $"Buy {quantity} {toBuy.name} ${Mathf.RoundToInt(itemCostBase*quantity)}";
    }
    public void OnClick()
    {
        if(RestaurantParameters.ins.TryBuyItem(itemCostBase*quantity))
        {
            RestaurantParameters.ins.AddToStorage(toBuy, quantity);
        }
    }
}
