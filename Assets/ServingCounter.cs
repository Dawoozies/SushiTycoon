using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;
using System;
public class ServingCounter : MonoBehaviour
{
    [SerializeField] int maxOrders;
    int nextFreeOrderIndex;
    [SerializeField] PolygonPositionArray orderPositions;
    float _spacesDistanceOrders;
    int nextFreeDishIndex;
    [SerializeField] PolygonPositionArray dishPositions;
    float _spacesDistanceDishes;
    [SerializeField, ReorderableList] List<Order> orders = new();
    //[SerializeField] SerializedDictionary<int, List<Order>> outstandingOrders = new();
    [SerializeField, ReorderableList] List<Dish> completedDishesReady = new();
    public void AddNewOrder(Order order)
    {
        orderPositions.RecomputePositions();

        orders.Add(order);

        nextFreeOrderIndex++;
        if (nextFreeOrderIndex >= orderPositions.positions.Length)
            nextFreeOrderIndex = 0;

        order.transform.parent = transform;
        order.transform.position = GetIncomingOrderPosition();
    }
    public void AddNewDish(Dish dish)
    {
        dishPositions.RecomputePositions();

        completedDishesReady.Add(dish);

        nextFreeDishIndex++;
        if (nextFreeDishIndex >= dishPositions.positions.Length)
            nextFreeDishIndex = 0;

        dish.transform.parent = transform;
        dish.transform.position = GetIncomingDishPosition();
    }
    public Vector2 GetIncomingOrderPosition()
    {
        Vector2 orderPosition;
        if(orderPositions.TryGetPositionAtIndex(nextFreeOrderIndex, out orderPosition))
        {
            return orderPosition;
        }
        else
        {
            orderPositions.TryGetPositionInQueue_Distance(0, out orderPosition);
        }

        return orderPosition;
    }
    public Vector2 GetIncomingDishPosition()
    {
        Vector2 dishPosition;
        if(dishPositions.TryGetPositionAtIndex(nextFreeDishIndex, out dishPosition))
        {
            return dishPosition;
        }
        else
        {
            dishPositions.TryGetPositionInQueue_Distance(0, out dishPosition);
        }
        return dishPosition;
    }
    public bool TryGetWork(ref Func<Dish> workFunc)
    {
        Debug.Log("Trying to get work");
        foreach (Order order in orders)
        {
            if(order.TryAssignWork(out workFunc))
            {
                return true;
            }
        }
        return false;
    }
    public bool TryGetCompletedDish(ref Func<Dish> workFunc)
    {
        workFunc = () => { return null; };
        if(completedDishesReady.Count == 0)
            return false;

        //we have to check if customer is still eating something else
        int dishToSendOut = 0;
        foreach (Dish completedDish in completedDishesReady)
        {
            if(completedDish.customerReadyForAnotherDish)
            {
                Dish dish = completedDish;
                workFunc = () => { return dish; };
                dishToSendOut = completedDishesReady.IndexOf(completedDish);
                completedDishesReady.RemoveAt(dishToSendOut);
                return true;
            }
        }
        return false;
    }
    public void OnBuild()
    {
        orderPositions.RecomputePositions();
        dishPositions.RecomputePositions();
    }
}