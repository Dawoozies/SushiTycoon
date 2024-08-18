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
        Eating,
        NeedsBill,
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
            case Task.Queue:
                textLines[0] = "Place In Queue";
                textLines[1] = fetchQueuePosition(this).queueNumber.ToString();
                break;
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
                    textLines[i+1] = dishNames[i];
                }
                break;
            case Task.Eating:
                textLines[0] = "Enjoying Succulent Meal";
                break;
            case Task.NeedsBill:
                textLines[0] = "Waiting For Bill";
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
                    CreateOrder();
                    currentTask = Task.HasDecidedOrder;
                    break;
                }
                break;
            case Task.HasDecidedOrder:

                break;
            case Task.Eating:
                break;
            case Task.NeedsBill:
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
        Warp(spawnPos);

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
    void CreateOrder()
    {
        List<DishData> pickedDishes = RestaurantParameters.ins.GetRandomMenuItems(RestaurantParameters.ins.GetRandomMenuOrderAmount());
        assignedTable.CreateUnfinishedOrder(this, pickedDishes, out awaitingOrder);
    }
}