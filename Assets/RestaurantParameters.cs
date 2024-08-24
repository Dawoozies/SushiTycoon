using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;
using System.Linq;
[ExecuteInEditMode]
public class RestaurantParameters : MonoBehaviour
{
    public static RestaurantParameters ins;

    private void Awake()
    {
        ins = this;
        Load();
        InitializeMenuDictionary();
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
    public float SingleDishCleaningTime;
    public float TotalCash;
    public float CashDisplayTime;
    public float OpenTime;
    public List<DishData> AllDishes = new();
    [SerializeField] SerializedDictionary<DishData, int> Menu = new();
    void InitializeMenuDictionary()
    {
        Menu.Clear();
        foreach (DishData dish in AllDishes)
        {
            Menu.Add(dish, dish.AmountAvailable());
        }
    }
    public void UpdateMenu()
    {
        foreach (DishData dish in AllDishes)
        {
            if (!Menu.ContainsKey(dish))
            {
                Menu.Add(dish, dish.AmountAvailable());
                continue;
            }
            Menu[dish] = dish.AmountAvailable();
        }
    }
    public bool TryGetRandomMenuItems(int amount, out List<DishData> pickedDishes)
    {
        UpdateMenu();
        List<DishData> keys = Menu.Keys.ToList();
        //pick randomly
        pickedDishes = new List<DishData>();
        int amountPicked = 0;
        while(amountPicked < amount)
        {
            if (keys == null || keys.Count == 0)
            {
                return false;
            }
            int i = Random.Range(0, keys.Count);
            if (Menu[keys[i]] <= 0)
            {
                keys.RemoveAt(i);
                continue;
            }

            amountPicked++;
            pickedDishes.Add(keys[i]);
            keys.RemoveAt(i);

            if (amountPicked >= amount)
                break;
        }
        
        foreach (DishData dish in pickedDishes)
        {
            dish.UseUpIngredients();
        }

        return true;
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
    public void CustomerPayBill(float billTotal)
    {
        TotalCash += billTotal;
    }
    public bool TryBuyItem(float itemCost)
    {
        float currentCash = TotalCash;
        if(currentCash - itemCost < 0)
        {
            return false;
        }
        TotalCash -= itemCost;
        return true;
    }
    public void SellItem(float itemCost)
    {
        TotalCash += itemCost;
    }
    public void OnEnable()
    {
        Load();
    }
    public void OnDisable()
    {
        if (Application.isEditor)
        {
            Save();
        }
    }
    public void OnApplicationQuit()
    {
        if (!Application.isEditor)
        {
            Save();
        }
    }
    void Save()
    {
        float totalCashSave = TotalCash;
        SaveGame.Save("TotalCash", totalCashSave);
    }
    void Load()
    {
        TotalCash = SaveGame.Load<float>("TotalCash");
    }
}
