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
    List<object> atTable = new();
    Action leaveTableAction;
    public enum State
    {
        Free,
        Full,
        Dirty,
    }
    private State state;

    Waiter assignedWaiter;
    Action breakWaiterAssignment;
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
        Tables.ins.RemoveTable(this);
    }
    void ChangeState(State newState)
    {
        Tables.ins.SwapState(this, state, newState);
        state = newState;
    }
    public bool TryAssignWaiter(Waiter waiter, ref Action breakWaiterAssignment)
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
}
public struct SeatingData
{
    public Vector2 position;
    public int seatNumber;
}