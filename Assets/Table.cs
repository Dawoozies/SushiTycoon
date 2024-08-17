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
    List<object> atTableList = new();

    public UnityEvent onSeatsFilled;
    public UnityEvent onTableCleared;
    public bool TryTakeSeat(object o, out Func<object, TableData> fetchTableData)
    {
        fetchTableData = null;
        if (atTableList.Count >= seatingPoints.Length)
            return false;
        fetchTableData = FetchTableData;
        atTableList.Add(o);

        if (atTableList.Count >= seatingPoints.Length)
            onSeatsFilled?.Invoke();
        
        return true;
    }
    public void LeaveTable(object o)
    {
        if(atTableList.Contains(o))
            atTableList.Remove(o);
    }
    TableData outgoingData;
    TableData FetchTableData(object o)
    {
        int seatNumber = atTableList.IndexOf(o);
        outgoingData.position = seatingPoints[seatNumber].position;
        outgoingData.seatNumber = seatNumber;
        return outgoingData;
    }
    public void AddToFreeTables()
    {
        Tables.ins.AddTable(this);
    }
    public void RemoveFromFreeTables()
    {
        Tables.ins.RemoveTable(this);
    }
}
public struct TableData
{
    public Vector2 position;
    public int seatNumber;
}