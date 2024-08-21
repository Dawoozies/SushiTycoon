using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using uPools;
public class Table : MonoBehaviour
{
    [ReorderableList] public Transform[] seatingPoints;
    [ReorderableList] public Transform[] platingPoints;
    [SerializeField] Transform orderPoint;
    List<object> atTable = new();
    Action leaveTableAction;
    public enum State
    {
        Free,
        Full,
        WaitingToOrder,
        WaitingForCompletedOrders,
        WaitingForBill,
        Dirty,
    }
    private State state;

    Waiter assignedWaiter;
    Action breakWaiterAssignment;

    [SerializeField] List<Order> outgoingOrders = new();
    public bool needsWaiter => outgoingOrders.Count > 0 && assignedWaiter == null;
    Action orderTakenCallback;
    float cleaningTime;
    public void OnRent()
    {
        atTable.Clear();
        //set up free table
        leaveTableAction = null;
    }
    public void OnBuild()
    {
        Tables.ins.AddTable(this, State.Free);
    }
    public void OnReturn()
    {
        //make things at table leave
        atTable.Clear();
        //remove from tables
        breakWaiterAssignment?.Invoke();
        breakWaiterAssignment = null;
        assignedWaiter = null;

        ////break seating assignments
        leaveTableAction?.Invoke();
        leaveTableAction = null;

        orderTakenCallback = null;

        ChangeState(State.Free);
        Tables.ins.RemoveTable(this);
    }
    void ChangeState(State newState)
    {
        Tables.ins.SwapState(this, state, newState);
        state = newState;
    }
    public bool TryAssignWaiter(Waiter waiter, Action breakWaiterAssignment)
    {
        if (assignedWaiter != null && assignedWaiter != waiter)
            return false;

        assignedWaiter = waiter;
        this.breakWaiterAssignment = breakWaiterAssignment;
        return true;
    }
    public bool TryAssignSeat(object o, ref Action leaveTableAction)
    {
        if (atTable.Count >= seatingPoints.Length)
            return false;

        atTable.Add(o);
        this.leaveTableAction += leaveTableAction;

        if(atTable.Count >= seatingPoints.Length)
        {
            Debug.LogError("Table full");
            ChangeState(State.Full);
            return true;
        }
        return false;
    }
    public bool TryGetSeatPosition(object o, out Vector2 seatPosition)
    {
        seatPosition = Vector2.zero;
        if (atTable == null || atTable.Count == 0)
            return false;
        if (!atTable.Contains(o))
            return false;

        int indexOfSeat = atTable.IndexOf(o);
        seatPosition = seatingPoints[indexOfSeat].position;
        return true;
    }
    public bool TryGetDishPosition(object o, out Vector2 dishPosition)
    {
        dishPosition = Vector2.zero;
        if (atTable == null || atTable.Count == 0)
            return false;
        if(!atTable.Contains(o))
            return false;

        int indexOfDish = atTable.IndexOf(o);
        dishPosition = platingPoints[indexOfDish].position;
        return true;
    }
    public void CreateUnfinishedOrder(Customer customer, List<DishData> pickedDishes, out Order awaitingOrder, Action orderTakenCallback)
    {
        awaitingOrder = SharedGameObjectPool.Rent(RestaurantParameters.ins.OrderPrefab).GetComponent<Order>();
        awaitingOrder.CustomerCreateOrder(customer, pickedDishes);
        outgoingOrders.Add(awaitingOrder);
        ChangeState(State.WaitingToOrder);
        this.orderTakenCallback += orderTakenCallback;
    }
    public bool TryTakeOrders(Waiter waiter, out List<Order> orders)
    {
        orders = null;
        if (assignedWaiter != null && assignedWaiter != waiter)
            return false;
        if (outgoingOrders == null || outgoingOrders.Count == 0)
            return false;

        orders = new List<Order>();
        foreach (Order order in outgoingOrders)
        {
            orders.Add(order);
        }

        orderTakenCallback?.Invoke();
        orderTakenCallback = null;

        outgoingOrders.Clear();

        //once order is taken remove waiter assignment from here
        assignedWaiter = null;
        breakWaiterAssignment = null;

        ChangeState(State.WaitingForCompletedOrders);

        return true;
    }
    public Vector2 GetOrderPoint()
    {
        return orderPoint.position;
    }
    List<Dish> dishesOnTable = new();
    public void SetTableDirty()
    {
        atTable.Clear();
        ChangeState(State.Dirty);
        cleaningTime = 0f;
        Dish[] dishes = GetComponentsInChildren<Dish>();
        foreach (var dish in dishes)
        {
            dishesOnTable.Add(dish);
        }
    }
    public bool CleanTable()
    {
        if(cleaningTime < RestaurantParameters.ins.SingleDishCleaningTime)
        {
            cleaningTime += Time.deltaTime;
        }
        else
        {
            if(dishesOnTable.Count > 0)
            {
                dishesOnTable[0].transform.parent = null;
                SharedGameObjectPool.Return(dishesOnTable[0].gameObject);
                dishesOnTable.RemoveAt(0);
            }
            cleaningTime = 0f;
        }

        if(dishesOnTable.Count > 0)
        {
            return false;
        }
        ChangeState(State.Free);
        return true;
    }
}
public struct SeatingData
{
    public Vector2 position;
    public int seatNumber;
}