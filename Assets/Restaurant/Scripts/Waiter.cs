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
    [SerializeField] WaiterTask currentTask;

    bool atSeat;

    [SerializeField]
    Transform[] arms;
    [SerializeField] Transform holdPoint;
    [SerializeField, Disable] Order _heldOrder;
    [SerializeField] TriggerVolumeEvents customerDetectionEvents;
    Customer orderingCustomer;

    float originalSpeed;
    protected override void Start()
    {
        base.Start();
        isIdle = true;
        pointNavigator = GetComponent<PointNavigator>();
        ActionTextPool.ins.Request(TaskProgressText);
        customerDetectionEvents.RegisterCollisionCallback(OnCustomerDetection, CollisionEventType.Stay);
    }
    protected override void Update()
    {
        base.Update();

        bool holdingSomething = holdPoint.childCount > 0;
        arms[0].gameObject.SetActive(!holdingSomething);
        arms[1].gameObject.SetActive(holdingSomething);

        if(currentTask == WaiterTask.Idle)
        {
            SetActiveNavigator(0);
        }
        else
        {
            SetActiveNavigator(1);
        }


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
                pointNavigator.SetPoint(orderingCustomer.transform.position);
                Order heldOrder;
                if(orderingCustomer.TryTakeOrder(this, out heldOrder))
                {
                    heldOrder.transform.parent = holdPoint;
                    heldOrder.transform.localPosition = Vector3.zero;
                    _heldOrder = heldOrder;
                    currentTask = WaiterTask.DeliveringOrder;
                }
                break;
            case WaiterTask.DeliveringOrder:
                pointNavigator.SetPoint(ServingCounter.ins.waiterPoint.position);
                if(Vector2.Distance(transform.position, ServingCounter.ins.waiterPoint.position) < RestaurantParameters.ins.ServingCounterAddOrderDistance)
                {
                    ServingCounter.ins.AddNewOrder(_heldOrder);
                    _heldOrder = null;
                    currentTask = WaiterTask.Idle;
                }
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
    public void OnCustomerDetection(Collider2D col)
    {
        Customer customer = col.GetComponentInParent<Customer>();
        if(customer != null && customer.readyToOrder && currentTask == WaiterTask.Idle)
        {
            if(customer.TryAssignOrderTakingWaiter(this))
            {
                currentTask = WaiterTask.TakingOrder;
                orderingCustomer = customer;
            }
        }
    }
}
public enum WaiterTask
{
    Idle, CleaningTable, TakingOrder, DeliveringOrder
}
