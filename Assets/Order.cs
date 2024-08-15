using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField, Disable] Waiter assignedWaiter;
    //List<DishData> order = new();
    [SerializeField, Disable] DishData order;
    [SerializeField, Disable] Customer customer;
    public bool AssignWaiter(Waiter waiter)
    {
        if (assignedWaiter != null && assignedWaiter != waiter)
            return false;
        assignedWaiter = waiter;
        return true;
    }
    public void CustomerCreateOrder(Customer customer, DishData order)
    {
        this.customer = customer;
        this.order = order;
    }
}
