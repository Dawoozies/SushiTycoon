using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dish : MonoBehaviour, IDish
{
    public DishState state => _state;
    private DishState _state;
    public DishData dishData => _dishData;

    public Chef assignedChef => throw new NotImplementedException();

    public Waiter assignedWaiter => throw new NotImplementedException();

    public Customer assignedCustomer => throw new NotImplementedException();

    private DishData _dishData;

    private PreperationStage prepStage;
    public void SetDishData(DishData dishData)
    {
        _dishData = dishData;
        _state = DishState.Empty;
    }
}

public interface IDish
{
    public DishState state { get; }
    public DishData dishData { get; }
    public void SetDishData(DishData dishData);
    public Chef assignedChef { get; }
    public Waiter assignedWaiter { get;}
    public Customer assignedCustomer { get; }
}

[Serializable]
public enum DishState
{
    Empty, RawIngredient, Preparing, Completed, DirtyPlate
}