using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaiterNavigator : NavigationSystem
{

    public bool isIdle;
    public Transform target;

    public Seat seat;

    PointNavigator pointNavigator;
    [SerializeField] WaiterTask currentTask;


    // Start is called before the first frame update
    bool atSeat;
    protected override void Start()
    {
        base.Start();
        isIdle = true;
        pointNavigator = GetComponent<PointNavigator>();
        //SetActiveNavigator(1);
        ActionTextPool.ins.Request(TaskProgressText);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();



        switch (currentTask)
        {
            case WaiterTask.Idle:
                seat = SeatManager.ins.RandomSeatWaiterNeeded();
                if (seat != null)
                {
                    if (seat.isDirty)
                    {
                        bool taskedSucceeded = seat.TryTaskWaiter(this);
                        if(taskedSucceeded)
                        {
                            pointNavigator.SetPoint(seat.GetDirtPosition());
                            currentTask = WaiterTask.CleaningTable;
                        }    
                    }
                    //if(seat.hasOrder) or some shit like that
                }
                break;
            case WaiterTask.CleaningTable:
                if(seat == null)
                {
                    currentTask = WaiterTask.Idle;
                    break;
                }
                atSeat = Vector2.Distance(transform.position, seat.transform.position) < RestaurantParameters.ins.SeatingDistance;
                if(atSeat)
                {
                    bool seatCleaned = seat.CleanSeat();
                    if(seatCleaned)
                    {
                        seat.ClearTaskWaiter(this);
                        seat = null;
                        currentTask = WaiterTask.Idle;
                        atSeat = false;
                    }
                }
                break;
            case WaiterTask.TakingOrder:
                break;
            case WaiterTask.DeliveringOrder:
                break;
        }
        /*if (isIdle)
        {
            seat = SeatManager.ins.GetDirtySeat();
            if (seat != null)
            {
                isIdle = false;
                pointNavigator.SetPoint(seat.dirtPosition);

            }
        }
        if (seat != null)
        {
            if (Vector2.Distance(transform.position, seat.dirtPosition) < SeatingParameters.ins.SeatingDistance)
            {
                currentTask = WaiterTask.CleaningTable;
                // Return to waiter queue if no new tasks
            }
        }*/

    }
    private void OnDisable()
    {
        if(seat != null)
        {
            seat.ClearTaskWaiter(this);
        }
    }
    ActionTextArgs taskProgressText = new();
    ActionTextArgs TaskProgressText()
    {
        taskProgressText.worldPos = transform.position + Vector3.up * 0.5f;
        string[] textLines = { "", "" };
        if (currentTask == WaiterTask.CleaningTable && seat != null && atSeat)
        {
            textLines[0] = "Cleaning";
            textLines[1] = $"{(int)Mathf.Lerp(100, 0, seat.dirtAmount)}%";
        }
        taskProgressText.textLines = textLines;
        return taskProgressText;
    }
}
public enum WaiterTask
{
    Idle, CleaningTable, TakingOrder, DeliveringOrder
}
