using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;
using System.Linq;
public class RestaurantParameters : MonoBehaviour
{
    public static RestaurantParameters ins;

    private void Awake()
    {
        ins = this;
        LoadResources();
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
    public int StaffUpperBound;

    public float SatisfactionGained;
    public float SatisfactionPerSecond;
    public float SatisfactionPerSecondThreshold;
    public float OpenTimeSeconds;
    public bool RestaurantOpen;
    public int ResearchPointsGained;
    public int ThresholdPerPoint;

    public List<DishData> AllDishes = new();
    [SerializeField] SerializedDictionary<DishData, int> Menu = new();
    [SerializeField] SerializedDictionary<CollectableData, int> ingredients = new();
    Dictionary<string, CollectableData> CollectableDataLookUp = new();
    Dictionary<string, int> ingredientSaveData = new();
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
        UpdateMenu();
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
    public void AddToStorage(CollectableData collectable, int amount)
    {
        if (!ingredients.TryAdd(collectable, amount))
        {
            ingredients[collectable] += amount;
        }

        if(!ingredientSaveData.TryAdd(collectable.name, amount))
        {
            ingredientSaveData[collectable.name] += amount;
        }

        UpdateMenu();
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
        ingredientSaveData[ingredient.name] -= requestAmount;

        return true;
    }
    public bool TryGetIngredientMultiple(CollectableData ingredient, int requiredAmount, out int multiple)
    {
        multiple = 0;
        if (!ingredients.ContainsKey(ingredient))
            return false;
        if (ingredients[ingredient] < requiredAmount)
            return false;
        if (requiredAmount == 0)
        {
            //if we dont need any ingredient amount to make this then
            //it is infinite source
            //making this 1 for now
            multiple = 999999;
            return true;
        }

        int amountInStorage = ingredients[ingredient];
        while (amountInStorage > 0)
        {
            amountInStorage -= requiredAmount;
            if (amountInStorage >= 0)
            {
                multiple++;
            }
        }
        return true;
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
        SaveGame.Save("ResearchPointsGained", ResearchPointsGained);
        SaveGame.Save<Dictionary<string, int>>("ingredientSaveData", ingredientSaveData);
    }
    void Load()
    {
        if(SaveGame.Exists("TotalCash"))
            TotalCash = SaveGame.Load<float>("TotalCash");
        if (SaveGame.Exists("ResearchPointsGained"))
            ResearchPointsGained = SaveGame.Load<int>("ResearchPointsGained");
        if(SaveGame.Exists("ingredientSaveData"))
        {
            ingredientSaveData = SaveGame.Load<Dictionary<string, int>>("ingredientSaveData");
            ingredients = new();
            foreach (string ingredientName in ingredientSaveData.Keys)
            {
                CollectableData ingredientData = CollectableDataLookUp[ingredientName];
                ingredients.Add(ingredientData, ingredientSaveData[ingredientName]);
            }
            UpdateMenu();
        }
    }
    void LoadResources()
    {
        CollectableData[] allCollectableData = Resources.LoadAll<CollectableData>("CollectableData");
        foreach (CollectableData collectableData in allCollectableData)
        {
            CollectableDataLookUp.TryAdd(collectableData.name, collectableData);
        }
    }
    public void ToggleRestaurant()
    {
        RestaurantOpen = !RestaurantOpen;
        SatisfactionGained = 0f;
    }
    void Update()
    {
        if(RestaurantOpen)
        {
            OpenTimeSeconds += Time.deltaTime;
        }
        else
        {
            OpenTimeSeconds = 0f;
            return;
        }

        SatisfactionPerSecondThreshold = Mathf.CeilToInt(10 + ResearchPointsGained * ThresholdPerPoint);
        if(Mathf.Approximately(OpenTimeSeconds,0f))
        {
            OpenTimeSeconds = 0.01f;
        }

        SatisfactionPerSecond = SatisfactionGained / OpenTimeSeconds;

        if(SatisfactionPerSecond >= SatisfactionPerSecondThreshold)
        {
            ResearchPointsGained++;
            SatisfactionGained = 0f;
        }
    }
    public void CustomerSatisfactionIncrease(float satisfaction)
    {
        SatisfactionGained += satisfaction;
    }
}
