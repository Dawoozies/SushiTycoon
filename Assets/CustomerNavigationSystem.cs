using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CustomerNavigationSystem : NavigationSystem
{
    //getting place in the queue
    //is entering restaurant
    Vector3 placeInQueue;
    Vector3 seatPosition;
    Vector3 despawnAreaPosition;
    Vector3 outsideMoveDirection;
    [SerializeField] CustomerTask currentTask;
    MoveInDirectionNavigator moveInDirectionNavigator;
    PointNavigator pointNavigator;
    protected override void Start()
    {
        base.Start();
        moveInDirectionNavigator = GetComponent<MoveInDirectionNavigator>();
        pointNavigator = GetComponent<PointNavigator>();
    }
    protected override void Update()
    {
        moveInDirectionNavigator.SetMoveDirection(outsideMoveDirection);
        base.Update();

        if(currentTask != CustomerTask.Outside)
        {
            // 0 = MoveInDirectionNavigator
            // 1 = PointNavigator
            SetActiveNavigator(1);
        }
        switch (currentTask)
        {
            case CustomerTask.Queueing:
                // calculate and set placeInQueue
                pointNavigator.SetPoint(placeInQueue);
                break;
            case CustomerTask.TakingSeat:
                // find and set seatPosition
                pointNavigator.SetPoint(seatPosition);
                break;
            case CustomerTask.Seated:
                pointNavigator.SetPoint(seatPosition);
                break;
            case CustomerTask.Leaving:
                pointNavigator.SetPoint(despawnAreaPosition);
                break;
        }
    }
    public void SetMoveDirection(Vector3 moveDir)
    {
        outsideMoveDirection = moveDir;
    }
}
[Serializable]
public enum CustomerTask
{
    Outside, Queueing, TakingSeat, Seated, Leaving
}