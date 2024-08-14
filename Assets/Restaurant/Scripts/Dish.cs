using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dish : MonoBehaviour, IDish
{

    public DishState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = DishState.Empty;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public DishState state { get; }
    private DishState _state;
}

public interface IDish
{
    public DishState state { get; }
}

[Serializable]
public enum DishState
{
    Empty, RawIngredient, Preparing, Completed, DirtyPlate
}