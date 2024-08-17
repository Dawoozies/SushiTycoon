using NavMeshPlus.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Customer : NavigationSystem
{
    //walking outside
    //going to queue
    public enum Task
    {
        WalkingOutside,
        Queue,
        Table,
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

    Func<object, TableData> fetchTableData;
    protected override void Start()
    {
        base.Start();

        despawnDetectionEvents.RegisterCollisionCallback(OnDespawnDetected, CollisionEventType.Enter);
        ActionTextPool.ins.Request(CustomerActionText);
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
                if(WaitingArea.ins.TryEnqueue(this, out fetchQueuePosition))
                {
                    currentTask = Task.Queue;
                }
                break;
            case Task.Queue:
                SetActiveNavigatorSpeed(queueSpeed);

                QueueData queueData = fetchQueuePosition(this);
                if (queueData.queueNumber < 0)
                {
                    //invalid queue position
                    LeaveQueue(Task.Leaving);
                    break;
                }

                if(atFrontOfQueue)
                {
                    Table table;
                    if(Tables.ins.TryGetRandomTable(out table))
                    {
                        if(table.TryTakeSeat(this, out fetchTableData))
                        {
                            LeaveQueue(Task.Table);
                            break;
                        }
                    }
                }

                pointNavigator.SetPoint(queueData.position);
                if (pointNavigator.nearDestination && !Patience())
                {
                    LeaveQueue(Task.Leaving);
                }
                break;
            case Task.Table:
                SetActiveNavigatorSpeed(walkingSpeed);
                Vector3 seatPos = fetchTableData(this).position;
                pointNavigator.SetPoint(seatPos);
                break;
            case Task.Leaving:
                SetActiveNavigatorSpeed(leavingSpeed);
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
        pointNavigator.SetPoint(spawner.DespawnerPositionRandom());
    }
}
