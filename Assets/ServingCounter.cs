using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServingCounter : MonoBehaviour
{
    public static ServingCounter ins;
    void Awake()
    {
        ins = this;
    }
    [SerializeField] List<Order> notFiredOrders = new();
    public void AddNewOrder(Order order)
    {
        notFiredOrders.Add(order);
    }
}