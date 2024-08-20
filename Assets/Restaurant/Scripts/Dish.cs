using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Dish : MonoBehaviour
{
    //[SerializeField, Disable] Chef assignedChef;
    [SerializeField, Disable] DishData dishData;
    [SerializeField, Disable] List<PreperationStage> unfinishedPrep = new();
    float stageTime;
    float eatingTime;
    public UnityEvent onDishCreated;
    Action<Dish> onDishCompleted;
    //should this only be initialized by order?
    Customer assignedCustomer;
    public Vector2 customerPos => assignedCustomer.transform.position;
    public bool customerReadyForAnotherDish => assignedCustomer.currentTask == Customer.Task.WaitingForOrder;
    Action<Dish> onDishEaten;
    public void InitializeDish(Customer assignedCustomer, DishData dishData, Action<Dish> onDishCompletedCallback)
    {
        unfinishedPrep.Clear();
        stageTime = 0f;
        eatingTime = 0f;
        this.dishData = dishData;
        foreach (PreperationStage prepStage in dishData.preparation)
        {
            unfinishedPrep.Add(prepStage);
        }
        this.assignedCustomer = assignedCustomer;
        onDishCompleted = onDishCompletedCallback;
        onDishCreated?.Invoke();
    }
    public bool TryPrepDish(out Vector2 prepPos, Vector2 handlerPos)
    {
        prepPos = Vector2.zero;
        if(unfinishedPrep.Count > 0)
        {
            Transform prepTransform;
            bool hasPrepStation = true;
            if (KitchenObjects.ins.TryGetClosestObjectWithID(transform.position, unfinishedPrep[0].requiredPrepStation, out prepTransform))
            {
                prepPos = prepTransform.position;
            }
            else
            {
                hasPrepStation = false;
            }

            if(hasPrepStation && Vector2.Distance(handlerPos, prepPos) < 0.2f)
            {
                if (stageTime < unfinishedPrep[0].stageTime)
                {
                    stageTime += Time.deltaTime;
                }
                else
                {
                    unfinishedPrep.RemoveAt(0);
                    stageTime = 0f;
                }
            }
            return false;
        }

        onDishCompleted?.Invoke(this);
        onDishCompleted = null;
        return true;
    }
    public string DishNameText()
    {
        return dishData.name;
    }
    public string PrepDescriptionText()
    {
        if (unfinishedPrep.Count == 0)
            return "Dish Complete!";
        return unfinishedPrep[0].prepStageDescription;
    }
    public bool TryEatingDish()
    {
        if(eatingTime < dishData.eatingTime)
        {
            eatingTime += Time.deltaTime;
            return false;
        }
        onDishEaten?.Invoke(this);
        onDishEaten = null;
        return true;
    }
    public void GiveCustomerCompletedDish()
    {
        assignedCustomer.GiveCompletedDish(this, TryEatingDish);
    }
    public void RegisterOnEatenCallback(Action<Dish> dishEatenAction)
    {
        onDishEaten += dishEatenAction;
    }
}