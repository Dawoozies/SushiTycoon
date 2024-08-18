using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class RestaurantParameters : MonoBehaviour
{
    public static RestaurantParameters ins;

    private void Awake()
    {
        ins = this;
    }

    public float SeatingDistance;
    public Vector2 PatienceBounds;
    public float CustomerSatisfactionDecay = 0.004f;
    public float CustomerEnterRestaurantCheckTime = 2f;
    public float CustomerEnterRestaurantChance;
    public Vector2 CustomerOrderTimeBounds;
    public Vector2Int CustomerMenuOrderAmountBounds;
    public GameObject OrderPrefab;
    public GameObject DishPrefab;
    public LayerMask AllBuiltObjectsLayerMask;
    public float ValidOrderTakingDistance = 0.75f;
    public float ServingCounterAddOrderDistance;
    public List<DishData> Menu;

    public DishData GetRandomMenuItem()
    {
        return Menu[Random.Range(0, Menu.Count)];
    }
    public List<DishData> GetRandomMenuItems(int amount)
    {
        List<DishData> menuItems = new List<DishData>();
        for (int i = 0; i < amount; i++)
        {
            menuItems.Add(GetRandomMenuItem());
        }
        return menuItems;
    }
    public float GetRandomOrderingTime()
    {
        return Random.Range(CustomerOrderTimeBounds.x, CustomerOrderTimeBounds.y);
    }
    public float GetRandomPatience()
    {
        return Random.Range(PatienceBounds.x, PatienceBounds.y);
    }
    public int GetRandomMenuOrderAmount()
    {
        return Random.Range(CustomerMenuOrderAmountBounds.x, CustomerMenuOrderAmountBounds.y+1);
    }
}
