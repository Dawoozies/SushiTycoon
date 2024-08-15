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
    [SerializeField]
    Transform[] orderPoints;
    public Transform waiterPoint;
    public Transform chefPoint;
    [SerializeField] List<Order> notFiredOrders = new();


    public void AddNewOrder(Order order)
    {
        notFiredOrders.Add(order);
        order.transform.parent = transform;
        UpdateNotFiredOrderPositions();
    }
    public Order FireOrder(int orderIndex)
    {
        Order firedOrder = notFiredOrders[orderIndex];
        notFiredOrders.RemoveAt(orderIndex);
        UpdateNotFiredOrderPositions();
        return firedOrder;
    }
    void UpdateNotFiredOrderPositions()
    {
        for (int i = 0; i < notFiredOrders.Count; i++)
        {
            Vector2 orderPoint = Vector2.Lerp(orderPoints[0].position, orderPoints[1].position, (float)i/ notFiredOrders.Count);
            notFiredOrders[i].transform.position = orderPoint;
        }
    }
}