using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Waiter : NavigationSystem
{
    PointNavigator pointNavigator;
    public enum Task
    {
        Waiting,
        TakingOrders,
        OrdersToKitchen,
        TakingCompletedOrderToTable,
        CleaningTable,
    }
    [SerializeField] Task currentTask;

    [SerializeField]
    Transform[] arms;
    [SerializeField] Transform holdPoint;
    [SerializeField, Disable] List<Order> heldOrders = new();

    Table assignedTable;
    ServingCounter servingCounter;
    bool nearestServingCounterFound;

    public float runningSpeed;
    Vector2 servingCounterOrderPos;
    protected override void Start()
    {
        base.Start();
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
                if (Tables.ins.TryGetRandomTable(Table.State.WaitingToOrder, out assignedTable))
                {
                    if(assignedTable.TryAssignWaiter(this, BreakWaiterTableAssignment))
                    {
                        currentTask = Task.TakingOrders;
                        break;
                    }
                }
                break;
            case Task.TakingOrders:
                SetActiveNavigatorSpeed(runningSpeed);
                pointNavigator.SetPoint(assignedTable.GetOrderPoint());
                if(Vector2.Distance(transform.position, assignedTable.GetOrderPoint()) < 0.2f)
                {
                    if(assignedTable.TryTakeOrders(this, out heldOrders))
                    {
                        foreach (var order in heldOrders)
                        {
                            order.AssignedWaiterPickUp(holdPoint);
                        }
                        currentTask = Task.OrdersToKitchen;
                        servingCounterOrderPos = servingCounter.GetIncomingOrderPosition();
                        break;
                    }
                }
                break;
            case Task.OrdersToKitchen:
                SetActiveNavigatorSpeed(runningSpeed);
                Debug.Log($"nearest serving counter found {nearestServingCounterFound}");

                if(nearestServingCounterFound)
                {
                    pointNavigator.SetPoint(servingCounterOrderPos);
                    if(Vector2.Distance(transform.position, servingCounterOrderPos) < 0.2f)
                    {
                        if(heldOrders.Count > 0)
                        {
                            servingCounter.AddNewOrder(heldOrders[0]);
                            heldOrders[0].AssignServingCounter(servingCounter);
                            heldOrders.RemoveAt(0);
                        }
                        else
                        {
                            currentTask = Task.Waiting;
                        }
                    }
                }
                break;
            case Task.TakingCompletedOrderToTable:
                break;
            case Task.CleaningTable:
                SetActiveNavigatorSpeed(runningSpeed);
                break;
        }
    }

    ActionTextArgs taskProgressText = new();
    ActionTextArgs TaskProgressText()
    {
        taskProgressText.worldPos = transform.position + Vector3.up * 0.5f;
        string[] textLines = { "", "" };
        taskProgressText.textLines = textLines;
        return taskProgressText;
    }
    void BreakWaiterTableAssignment()
    {
        currentTask = Task.Waiting;
        assignedTable = null;
    }
}
