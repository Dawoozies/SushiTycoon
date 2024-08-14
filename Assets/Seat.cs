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
    public WaiterNavigator taskedWaiter;
    bool hasTaskedWaiter;
    public float dirtAmount;
    [Serializable] public enum SeatingDirection
    {
        Left, Right, Up, Down
    }

    // Start is called before the first frame update
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

    public void LeaveSeat()
    {
        isSeated = false;
        isOccupied = false;
        isDirty = true;
        dirtAmount = 1;
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
    public bool TryTaskWaiter(WaiterNavigator waiter)
    {
        if (hasTaskedWaiter)
            return false;

        taskedWaiter = waiter;
        hasTaskedWaiter = true;
        return true;
    }
    public void ClearTaskWaiter(WaiterNavigator waiter)
    {
        if(taskedWaiter == waiter)
        {
            taskedWaiter = null;
            hasTaskedWaiter = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
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

}
