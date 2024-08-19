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

    //the key is how much prep is left for the order
    //chefs will prioritise orders which can be finished quicker
    [SerializeField] SerializedDictionary<int, List<Order>> outstandingOrders = new();
    private void Awake()
    {
        orderPositions.RecomputePositions();
        dishPositions.RecomputePositions();
    }
    public void AddNewOrder(Order order)
    {
        orderPositions.RecomputePositions();

        order.transform.parent = transform;
        order.transform.position = GetIncomingOrderPosition();

        AddOutstandingOrder(order, order.GetDishesToComplete());

        nextFreeOrderIndex++;
        if (nextFreeOrderIndex >= 100)
            nextFreeOrderIndex = 0;
    }
    public void AddNewDish(Dish dish)
    {
        dishPositions.RecomputePositions();

        dish.transform.parent = transform;
        dish.transform.position = GetIncomingDishPosition();

        nextFreeDishIndex++;
        if (nextFreeDishIndex >= 100)
            nextFreeDishIndex = 0;
    }
    public Vector2 GetIncomingOrderPosition()
    {
        Vector2 orderPosition;
        if(orderPositions.TryGetPositionAtIndex(nextFreeOrderIndex, out orderPosition))
        {
            return orderPosition;
        }

        return Vector2.zero;
    }
    public Vector2 GetIncomingDishPosition()
    {
        Vector2 dishPosition;
        if(dishPositions.TryGetPositionAtIndex(nextFreeDishIndex, out dishPosition))
        {
            return dishPosition;
        }
        return Vector2.zero;
    }
    public void UpdateDishesLeftForOrder(Order order, int oldDishesLeftToComplete, int newDishesLeftToComplete)
    {
        if(outstandingOrders.ContainsKey(oldDishesLeftToComplete))
        {
            if (outstandingOrders[oldDishesLeftToComplete].Contains(order))
                outstandingOrders[oldDishesLeftToComplete].Remove(order);
        }
        if(!outstandingOrders.ContainsKey(newDishesLeftToComplete))
        {
            AddOutstandingOrder(order, newDishesLeftToComplete);
            return;
        }
        outstandingOrders[newDishesLeftToComplete].Add(order);
    }
    public void AddOutstandingOrder(Order order, int dishesToComplete)
    {
        if(outstandingOrders.ContainsKey(dishesToComplete))
        {
            outstandingOrders[dishesToComplete].Add(order);
        }
        else
        {
            outstandingOrders.Add(dishesToComplete, new List<Order> { order });
        }
    }
    public bool TryAssignChefToAnOutstandingOrder(Chef assignedChef, out Dish dish, out Order order, out Action<Chef, Dish> dishCompleteCallback)
    {
        dish = null;
        order = null;
        dishCompleteCallback = null;

        //look at outstanding orders
        //if (outstandingOrders.Keys.Count == 0)
        //    return false;
        //if (outstandingOrders.Keys.Count == 1 && outstandingOrders.ContainsKey(0))
        //    return false;

        foreach (int key in outstandingOrders.Keys)
        {
            if (key == 0)
            {
                Debug.Log($"key = {key}");
                continue;
            }
            //the next one will straight up be the smallest ones
            foreach (Order outstandingOrder in outstandingOrders[key])
            {
                if(outstandingOrder.TryAssignChefToDish(assignedChef, ref dish, ref order, ref dishCompleteCallback))
                {
                    //must terminate in here
                    Debug.LogError($"dish{dish.DishNameText()}  order{order.name} key = {key}");
                    return true;
                }
            }
            Debug.Log($"key = {key}");
        }
        return false;
    }
    //public bool TryAssignWaiterToCompletedDish(Waiter assignedWaiter, out Dish dish, out Order order, out Action<Waiter, Dish, Order> dishDeliveredCallback)
    //{
    //    dish = null;
    //    order = null;
    //    dishDeliveredCallback = null;
    //}
}