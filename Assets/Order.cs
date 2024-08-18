using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Order : MonoBehaviour
{
    [SerializeField, Disable] Waiter assignedWaiter;
    [SerializeField, Disable] List<DishData> order = new();
    [SerializeField, Disable] Customer customer;
    public UnityEvent onOrderCreated;
    public UnityEvent onWaiterPickUp;
    public bool AssignWaiter(Waiter waiter)
    {
        if (assignedWaiter != null && assignedWaiter != waiter)
            return false;
        assignedWaiter = waiter;
        return true;
    }
    public void CustomerCreateOrder(Customer customer, List<DishData> order)
    {
        this.customer = customer;
        this.order = order;
        onOrderCreated?.Invoke();
    }
    public string[] GetDishNames()
    {
        string[] dishNames = new string[order.Count];
        for (int i = 0; i < order.Count; i++)
        {
            dishNames[i] = order[i].name;
        }
        return dishNames;
    }
    public bool TryAssignedWaiterPickUp(Waiter waiter, Transform holdPoint)
    {
        if(assignedWaiter != waiter)
            return false;
        transform.parent = holdPoint;
        transform.localPosition = Vector3.zero;
        onWaiterPickUp?.Invoke();
        return true;
    }
}
