using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Waiter : NavigationSystem
{

    public bool isIdle;
    public Transform target;

    public Seat seat;

    PointNavigator pointNavigator;
    public enum Task
    {
        Waiting,
        TakingOrders,
        OrdersToKitchen,
        TakingCompletedOrderToTable,
        CleaningTable,
    }
    //service table means do anything the table needs
    [SerializeField] Task currentTask;

    bool atSeat;

    [SerializeField]
    Transform[] arms;
    [SerializeField] Transform holdPoint;
    [SerializeField, Disable] Order _heldOrder;
    Customer orderingCustomer;

    float originalSpeed;
    protected override void Start()
    {
        base.Start();
        isIdle = true;
        pointNavigator = GetComponent<PointNavigator>();
        ActionTextPool.ins.Request(TaskProgressText);
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


        switch (currentTask)
        {
            case Task.Waiting:
                break;
            case Task.TakingOrders:
                break;
            case Task.OrdersToKitchen:
                break;
            case Task.TakingCompletedOrderToTable:
                break;
            case Task.CleaningTable:
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
        if (currentTask == Task.CleaningTable && seat != null && atSeat)
        {
            textLines[0] = "Cleaning";
            textLines[1] = $"{(int)Mathf.Lerp(100, 0, seat.dirtAmount)}%";
        }
        taskProgressText.textLines = textLines;
        return taskProgressText;
    }
}
