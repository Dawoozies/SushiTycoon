using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dish : MonoBehaviour, IDish
{
    public DishState state => _state;
    private DishState _state;
    public DishData dishData => _dishData;
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
}

[Serializable]
public enum DishState
{
    Empty, RawIngredient, Preparing, Completed, DirtyPlate
}