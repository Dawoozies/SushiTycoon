using NavMeshPlus.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using uPools;
public class Customer : NavigationSystem
{
    //walking outside
    //going to queue
    public enum Task
    {
        WalkingOutside,
        Queue,
        Table,
        LookingAtMenu,
        HasDecidedOrder,
        WaitingForOrder,
        Eating,
        Leaving,
    }
    public Task currentTask;
    public float walkingSpeed;
    public float queueSpeed;
    public float leavingSpeed;
    MoveInDirectionNavigator moveInDirectionNavigator;
    PointNavigator pointNavigator;
    CustomerSpawner spawner;
    [SerializeField] TriggerVolumeEvents despawnDetectionEvents;
    float patience;
    Func<object, QueueData> fetchQueuePosition;
    ActionTextArgs actionTextArgs = new();
    bool atFrontOfQueue { get {
            if (currentTask != Task.Queue)
                return false;
            if (fetchQueuePosition == null)
                return false;
            QueueData queueData = fetchQueuePosition(this);
            int queueNumber = queueData.queueNumber;
            return queueNumber == 0;
        } }
    Table assignedTable;
    Action leaveTableAction;

    float orderTime;
    Order awaitingOrder;
    List<Func<bool>> eatingFuncs = new();
    float totalOrderPrice;
    protected override void Start()
    {
        base.Start();

        despawnDetectionEvents.RegisterCollisionCallback(OnDespawnDetected, CollisionEventType.Enter);
        ActionTextPool.ins.Request(CustomerActionText);
        leaveTableAction = LeaveTable;
    }
    ActionTextArgs CustomerActionText()
    {
        actionTextArgs.worldPos = transform.position + Vector3.up * 0.2f;
        string[] textLines = { "", "" };
        switch (currentTask)
        {
            case Task.WalkingOutside:
                break;
            //case Task.Queue:
            //    textLines[0] = "Place In Queue";
            //    textLines[1] = fetchQueuePosition(this).queueNumber.ToString();
            //    break;
            case Task.Table:
                textLines[0] = "Taking Seat At Table";
                break;
            case Task.Leaving:
                textLines[0] = "Leaving";
                break;
            case Task.LookingAtMenu:
                textLines[0] = "Looking At Menu";
                break;
            case Task.HasDecidedOrder:
                string[] dishNames = awaitingOrder.GetDishNames();
                textLines = new string[dishNames.Length + 1];
                textLines[0] = "Order:";
                for (int i = 0; i < dishNames.Length; i++)
                {
                    textLines[i + 1] = dishNames[i];
                }
                break;
            case Task.Eating:
                textLines[0] = "Enjoying Succulent Meal";
                break;
        }
        actionTextArgs.textLines = textLines;
        return actionTextArgs;
    }
    protected override void Update()
    {
        base.Update();
        if (currentTask == Task.WalkingOutside)
        {
            SetActiveNavigator(0);
        }
        else
        {
            SetActiveNavigator(1);
        }

        switch (currentTask)
        {
            case Task.WalkingOutside:
                SetActiveNavigatorSpeed(walkingSpeed);
                if (WaitingArea.ins.TryEnqueue(this, out fetchQueuePosition))
                {
                    currentTask = Task.Queue;
                }
                break;
            case Task.Queue:
                SetActiveNavigatorSpeed(queueSpeed);
                if (atFrontOfQueue)
                {
                    //try get table
                    if (Tables.ins.TryGetRandomTable(Table.State.Free, out assignedTable))
                    {
                        if (assignedTable.TryAssignSeat(this, ref leaveTableAction))
                        {
                            LeaveQueue(Task.Table);
                        }
                        break;
                    }
                }

                QueueData queueData = fetchQueuePosition(this);
                if (queueData.queueNumber < 0)
                {
                    //invalid queue position
                    LeaveQueue(Task.Leaving);
                    break;
                }

                pointNavigator.SetPoint(queueData.position);
                if (pointNavigator.nearPoint && !Patience())
                {
                    LeaveQueue(Task.Leaving);
                }
                break;
            case Task.Table:
                SetActiveNavigatorSpeed(walkingSpeed);
                Vector2 seatPosition;
                if (assignedTable.TryGetSeatPosition(this, out seatPosition))
                {
                    pointNavigator.SetPoint(seatPosition);
                    if(pointNavigator.nearPoint)
                    {
                        currentTask = Task.LookingAtMenu;
                        orderTime = RestaurantParameters.ins.GetRandomOrderingTime();
                    }
                }
                break;
            case Task.Leaving:
                SetActiveNavigatorSpeed(leavingSpeed);
                break;
            case Task.LookingAtMenu:
                if(orderTime > 0)
                {
                    orderTime -= Time.deltaTime;
                }
                else
                {
                    if(TryCreateOrderToTable())
                    {
                        currentTask = Task.HasDecidedOrder;
                    }
                    else
                    {
                        LeaveTableNoFoodOnMenu();
                    }
                    break;
                }
                break;
            case Task.HasDecidedOrder:
                break;
            case Task.WaitingForOrder:
                break;
            case Task.Eating:
                if(eatingFuncs.Count > 0)
                {
                    if (eatingFuncs[0]())
                    {
                        eatingFuncs.RemoveAt(0);
                    }
                }
                break;
        }
    }
    public void OnSpawn(CustomerSpawner spawner, Vector2 spawnPos, Vector2 moveDir)
    {
        currentTask = Task.WalkingOutside;
        if(moveInDirectionNavigator == null)
            moveInDirectionNavigator = GetComponent<MoveInDirectionNavigator>();
        if(pointNavigator == null)
            pointNavigator = GetComponent<PointNavigator>();
        this.spawner = spawner;
        moveInDirectionNavigator.SetMoveDirection(moveDir);
        transform.position = spawnPos;
        //Warp(spawnPos);
        patience = RestaurantParameters.ins.GetRandomPatience();
    }
    void OnDespawnDetected(Collider2D despawnCollider)
    {
        LeaveQueue(Task.WalkingOutside);
        spawner.Return(gameObject);
    }
    bool Patience()
    {
        if (patience > 0)
        {
            patience -= Time.deltaTime;
            return true;
        }

        return false;
    }
    void LeaveQueue(Task nextTask)
    {
        currentTask = nextTask;
        fetchQueuePosition = null;
        WaitingArea.ins.LeaveQueue(this);
        if(nextTask == Task.Leaving)
            pointNavigator.SetPoint(spawner.DespawnerPositionRandom());
    }
    void LeaveTable()
    {
        assignedTable = null;
        currentTask = Task.Leaving;
        pointNavigator.SetPoint(spawner.DespawnerPositionRandom());
    }
    void LeaveTableNoFoodOnMenu()
    {
        assignedTable.LeaveTableEarly();
        assignedTable = null;
        currentTask = Task.Leaving;
        pointNavigator.SetPoint(spawner.DespawnerPositionRandom());
    }
    bool TryCreateOrderToTable()
    {
        List<DishData> pickedDishes;
        if(!RestaurantParameters.ins.TryGetRandomMenuItems(RestaurantParameters.ins.GetRandomMenuOrderAmount(), out pickedDishes))
        {
            return false;
        }

        totalOrderPrice = 0f;
        foreach (DishData pickedDish in pickedDishes)
        {
            totalOrderPrice += pickedDish.priceBase;
        }
        assignedTable.CreateUnfinishedOrder(this, pickedDishes, out awaitingOrder, OrderTakenCallback);
        return true;
    }
    void OrderTakenCallback()
    {
        Debug.Log("Customer waiting for order now");
        currentTask = Task.WaitingForOrder;
    }
    public void GiveCompletedDish(Dish completedDish, Func<bool> eatingFunc)
    {
        Vector2 dishPos;
        if(assignedTable.TryGetDishPosition(this, out dishPos))
        {
            completedDish.transform.parent = assignedTable.transform;
            completedDish.transform.position = dishPos;
            eatingFuncs.Add(eatingFunc);
            currentTask = Task.Eating;
        }
    }
    public void LastDishOfOrderEaten(Order order)
    {
        if(awaitingOrder == order)
        {
            //THEN WE HAVE FINISHED EATING!!!
            assignedTable.SetTableDirty();
            awaitingOrder = null;
            RestaurantParameters.ins.CustomerPayBill(totalOrderPrice);
            AnimatedTextPool.ins.Request(transform.position + Vector3.up*0.2f, $"+ ${totalOrderPrice}");
            totalOrderPrice = 0f;
            LeaveTable();
        }
    }
    public void DishOfOrderEatenButOrderNotFinished()
    {
        if(eatingFuncs.Count == 0)
        {
            currentTask = Task.WaitingForOrder;
        }
    }
}