using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Chef : NavigationSystem
{
    PointNavigator pointNavigator;
    public enum Task
    {
        Waiting,
        GetOrderFromServingCounter,
        CookingDish,
        MovingDishToServingCounter,
    }
    [SerializeField] Task currentTask;
    [SerializeField] Transform[] arms;
    [SerializeField] Transform holdPoint;

    [SerializeField,Disable] ServingCounter servingCounter;
    [SerializeField, Disable] bool nearestServingCounterFound;
    //we complete a dish we use the callback to send it to the order as completed
    [SerializeField, Disable] Order assignedOrder;
    [SerializeField, Disable] Dish assignedDish;
    Action<Chef, Dish> dishCompleteCallback;
    Vector2 servingCounterDishPos;
    protected override void Start()
    {
        base.Start();
        pointNavigator = GetComponent<PointNavigator>();
        ActionTextPool.ins.Request(TaskProgressText);
    }

    ActionTextArgs taskProgressText = new();
    ActionTextArgs TaskProgressText()
    {
        taskProgressText.worldPos = transform.position + Vector3.up * 0.5f;
        string[] textLines = { "", "" };
        switch (currentTask)
        {
            case Task.Waiting:
                break;
            case Task.GetOrderFromServingCounter:
                break;
            case Task.CookingDish:
                //display what prep is happening
                textLines[0] = $"Cooking Dish : {assignedDish.DishNameText()}";
                textLines[1] = assignedDish.PrepDescriptionText();
                break;
        }
        taskProgressText.textLines = textLines;
        return taskProgressText;
    }
    protected override void Update()
    {
        base.Update();
        bool holdingSomething = holdPoint.childCount > 0;
        arms[0].gameObject.SetActive(!holdingSomething);
        arms[1].gameObject.SetActive(holdingSomething);
        if(currentTask == Task.Waiting)
        {
            SetActiveNavigator(0);
        }
        else
        {
            SetActiveNavigator(1);
        }
        if (!nearestServingCounterFound)
        {
            if (KitchenObjects.ins.TryGetClosestObjectWithID(transform.position, KitchenObject.ObjectID.ServingCounter, out servingCounter))
            {
                nearestServingCounterFound = true;
            }
        }
        switch (currentTask)
        {
            case Task.Waiting:
                if(nearestServingCounterFound)
                {
                    if(servingCounter.TryAssignChefToAnOutstandingOrder(this, out assignedDish, out assignedOrder, out dishCompleteCallback))
                    {
                        currentTask = Task.CookingDish;
                        assignedDish.transform.parent = holdPoint;
                        assignedDish.transform.localPosition = Vector2.zero;
                    }
                }
                break;
            case Task.CookingDish:
                if(assignedDish.TryPrepDish())
                {
                    currentTask = Task.MovingDishToServingCounter;
                    servingCounterDishPos = servingCounter.GetIncomingDishPosition();
                }
                break;
            case Task.MovingDishToServingCounter:
                pointNavigator.SetPoint(servingCounterDishPos);
                if(Vector2.Distance(transform.position, servingCounterDishPos) < 0.2f)
                {
                    servingCounter.AddNewDish(assignedDish);
                    dishCompleteCallback?.Invoke(this, assignedDish);
                    assignedDish = null;
                    assignedOrder = null;
                    currentTask = Task.Waiting;
                }
                break;
        }
    }
}
