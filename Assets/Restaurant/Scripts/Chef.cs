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

    [SerializeField, Disable] ServingCounter servingCounter;
    [SerializeField, Disable] bool nearestServingCounterFound;

    Func<Dish> workFunc;
    [SerializeField] float runningSpeed;
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
                //textLines[0] = $"Cooking Dish : {assignedDish.DishNameText()}";
                //textLines[1] = assignedDish.PrepDescriptionText();
                textLines[0] = $"Cooking";
                textLines[1] = workFunc().PrepDescriptionText();
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
                    //get the goddamn dish
                    if(servingCounter.TryGetWork(ref workFunc))
                    {
                        currentTask = Task.CookingDish;
                        workFunc().transform.parent = holdPoint;
                        workFunc().transform.localPosition = Vector3.zero;
                        Debug.Log("Got work func properly");
                        break;
                    }
                }
                break;
            case Task.CookingDish:
                pointNavigator.SetSpeed(runningSpeed);
                Vector2 prepPosition;
                if(workFunc().TryPrepDish(out prepPosition, transform.position))
                {
                    currentTask = Task.MovingDishToServingCounter;
                    Debug.LogError("COMPLETED DISH");
                }
                pointNavigator.SetPoint(prepPosition);
                break;
            case Task.MovingDishToServingCounter:
                pointNavigator.SetSpeed(runningSpeed);
                pointNavigator.SetPoint(servingCounter.GetIncomingDishPosition());
                if (Vector2.Distance(transform.position, servingCounter.GetIncomingDishPosition()) < 0.125f)
                {
                    servingCounter.AddNewDish(workFunc());
                    workFunc = null;
                    currentTask = Task.Waiting;
                }
                break;
        }
    }
}
