using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Seat : MonoBehaviour
{
    public SeatingDirection seatingDirection;
    public bool isOccupied; // True as soon as a customer claims an empty seat
    public bool isDirty;
    public bool isSeated; // True only when a customer is seated
    public bool waiterNeeded => isDirty;
    Vector2 seatDirection;
    GameObject dirt;
    public Vector3 dirtPosition;
    public Waiter taskedWaiter;
    bool hasTaskedWaiter;
    public float dirtAmount;
    [Serializable] public enum SeatingDirection
    {
        Left, Right, Up, Down
    }

    Customer assignedCustomer;
    void Start()
    {
        isOccupied = false;
        isDirty = false;
        isSeated = false;
        dirt = gameObject.transform.GetChild(0).gameObject;
        dirtPosition = transform.position + SeatingDirectionToVector() * RestaurantParameters.ins.SeatingDistance;
        dirt.transform.position = dirtPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDirty)
        {
            if(dirt.activeInHierarchy == false)
            {
                SeatManager.ins.dirtySeats.Add(this);
            }
        }
        dirt.SetActive(isDirty);
    }

    public Vector3 GetDirtPosition()
    {
        return dirtPosition;
    }

    public void LeaveSeat(Customer customer)
    {
        if(assignedCustomer == customer)
        {
            isSeated = false;
            isOccupied = false;
            isDirty = true;
            dirtAmount = 1;
            assignedCustomer = null;
        }
    }

    public bool CleanSeat()
    {
        if(dirtAmount > 0)
        {
            dirtAmount -= Time.deltaTime;
            return false;
        }

        dirtAmount = 0;
        isDirty = false;
        return true;
    }
    public bool TryTaskWaiter(Waiter waiter)
    {
        if (hasTaskedWaiter)
            return false;

        taskedWaiter = waiter;
        hasTaskedWaiter = true;
        return true;
    }
    public void ClearTaskWaiter(Waiter waiter)
    {
        if(taskedWaiter == waiter)
        {
            taskedWaiter = null;
            hasTaskedWaiter = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + SeatingDirectionToVector() * RestaurantParameters.ins.SeatingDistance);
    }
    Vector3 SeatingDirectionToVector()
    {
        switch (seatingDirection)
        {
            case SeatingDirection.Left:
                return Vector3.left;
            case SeatingDirection.Right:
                return Vector3.right;
            case SeatingDirection.Up:
                return Vector3.up;
            case SeatingDirection.Down:
                return Vector3.down;
        }
        return Vector3.zero;
    }
    public void AddToSeatsList()
    {
        SeatManager.ins.AddSeat(this);
    }
    public void RemoveFromSeatsList()
    {
        SeatManager.ins.RemoveSeat(this);
    }
    public bool TryCustomerSeat(Customer customer)
    {
        if (assignedCustomer != null)
            return false;
        assignedCustomer = customer;
        return true;
    }
}
