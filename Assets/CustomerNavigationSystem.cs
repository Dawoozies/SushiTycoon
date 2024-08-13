using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CustomerNavigationSystem : NavigationSystem
{
    //getting place in the queue
    //is entering restaurant
   [SerializeField] Vector3 positionInQueue;
    [SerializeField] int placeInQueue;
    Vector3 seatPosition;
    [SerializeField] public Vector3 despawnAreaPosition;
    Vector3 outsideMoveDirection;
    [SerializeField] CustomerTask currentTask;
    MoveInDirectionNavigator moveInDirectionNavigator;
    PointNavigator pointNavigator;
    NavMeshAgent agent;
    Seat seat;
    float customerSpeedFull;
    public float inQueueSpeed;
    public float slowDistance;

    public float queuePatience;

    protected override void Start()
    {
        base.Start();
        moveInDirectionNavigator = GetComponent<MoveInDirectionNavigator>();
        pointNavigator = GetComponent<PointNavigator>();
        agent = GetComponent<NavMeshAgent>();
        placeInQueue = -1; // Not yet in queue

        customerSpeedFull = agent.speed;
        inQueueSpeed = customerSpeedFull / 8f;
        slowDistance = 2f;
        queuePatience = UnityEngine.Random.Range(10, 120); // Low numbers just for testing. Tweak
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
                queuePatience -= Time.deltaTime;
                if (queuePatience < 0) currentTask = CustomerTask.Leaving;

                if(placeInQueue == 0)
                {
                    seat = SeatManager.ins.RandomAvailableSeat();
                    if (seat != null)
                    {
                        currentTask = CustomerTask.TakingSeat;
                    }
                }

                // calculate and set positionInQueue
                //placeInQueue 

                positionInQueue = QueueSystem.ins.JoinQueue(placeInQueue, out int newPosition);
                if (Vector3.Distance(transform.position, positionInQueue) < QueueSystem.ins.queueSize * slowDistance)
                {
                    agent.speed = inQueueSpeed;
                } else
                {
                    agent.speed = customerSpeedFull;
                }

                pointNavigator.SetPoint(positionInQueue);
                placeInQueue = newPosition;
                break;
            case CustomerTask.TakingSeat:
                // Make sure customer isn't still slowed from queue
                if (placeInQueue >= 0)
                {
                    QueueSystem.ins.positionOccupied[placeInQueue] = false;
                    placeInQueue = -1;
                    agent.speed = customerSpeedFull;
                    seatPosition = seat.transform.position;
                    pointNavigator.SetPoint(seatPosition);
                }
                //If at seat, switch to Seated state
                Debug.Log(Vector3.Distance(transform.position, seatPosition));
                if (Vector3.Distance(transform.position, seatPosition) < SeatingParameters.ins.SeatingDistance)
                {
                    currentTask = CustomerTask.Seated;
                }
                // find and set seatPosition
                break;
            case CustomerTask.Seated:
                pointNavigator.SetPoint(seatPosition);
                seat.isSeated = true;
                break;
            case CustomerTask.Leaving:

                if(seat != null)
                {
                    seat.LeaveSeat();
                    seat = null;

                }
                // Make sure customer isn't still slowed from queue
                if (placeInQueue >= 0)
                {
                    QueueSystem.ins.positionOccupied[placeInQueue] = false;

                }
                placeInQueue = -1;
                agent.speed = customerSpeedFull;
                pointNavigator.SetPoint(despawnAreaPosition);
                break;
        }
    }
    public void SetMoveDirection(Vector3 moveDir)
    {
        outsideMoveDirection = moveDir;
    }

    private void OnValidate()
    {
        if(currentTask == CustomerTask.Leaving)
        {
            if (placeInQueue >= 0) QueueSystem.ins.positionOccupied[placeInQueue] = false;



        }
    }
}
[Serializable]
public enum CustomerTask
{
    Outside, Queueing, TakingSeat, Seated, Leaving
}