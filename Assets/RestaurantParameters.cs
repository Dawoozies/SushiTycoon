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
    public float GetRandomOrderingTime()
    {
        return Random.Range(CustomerOrderTimeBounds.x, CustomerOrderTimeBounds.y);
    }
    public float GetRandomPatience()
    {
        return Random.Range(PatienceBounds.x, PatienceBounds.y);
    }
}
