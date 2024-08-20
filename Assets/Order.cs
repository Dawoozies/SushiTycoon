using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using uPools;
public class Order : MonoBehaviour
{
    [SerializeField, Disable] Waiter assignedWaiter;
    [SerializeField, Disable] List<DishData> dishList = new();
    [SerializeField, Disable] Customer customer;
    public UnityEvent onOrderCreated;
    public UnityEvent onWaiterPickUp;
    public void CustomerCreateOrder(Customer customer, List<DishData> dishes)
    {
        Debug.Log("Customer create order");
        this.customer = customer;
        dishList = new List<DishData>();
        foreach (DishData dish in dishes)
        {
            dishList.Add(dish);
        }

        unassignedWork.Clear();
        for (int i = 0; i < dishList.Count; i++)
        {
            unassignedWork.Add(i);
        }
        onOrderCreated?.Invoke();

    }
    public List<int> unassignedWork = new();
    public List<Dish> dishes = new List<Dish>();
    public bool TryAssignWork(out Func<Dish> fetchDishFunc)
    {
        fetchDishFunc = () => { return null; };
        if (unassignedWork.Count == 0)
            return false;

        int toAssignIndex = unassignedWork[0];
        DishData dataToAssign = dishList[toAssignIndex];
        Dish newDishAssignment = SharedGameObjectPool.Rent(RestaurantParameters.ins.DishPrefab).GetComponent<Dish>();

        newDishAssignment.InitializeDish(customer, dataToAssign, OnDishCompleted);
        newDishAssignment.RegisterOnEatenCallback(OnDishEaten);

        dishes.Add(newDishAssignment);

        fetchDishFunc = () => { return newDishAssignment; };

        unassignedWork.RemoveAt(0);
        return true;
    }
    void OnDishCompleted(Dish dishCompleted)
    {
    }
    public string[] GetDishNames()
    {
        string[] dishNames = new string[dishList.Count];
        for (int i = 0; i < dishList.Count; i++)
        {
            dishNames[i] = dishList[i].name;
        }
        return dishNames;
    }
    public void WaiterPickUp(Transform holdPoint)
    {
        transform.parent = holdPoint;
        transform.localPosition = Vector3.zero;
        onWaiterPickUp?.Invoke();
    }
    void OnDishEaten(Dish dishEaten)
    {
        if(dishes.Contains(dishEaten))
        {
            dishes.Remove(dishEaten);
            if(dishes.Count == 0)
            {
                customer.LastDishOfOrderEaten(this);
                SharedGameObjectPool.Return(gameObject);
            }
            else
            {
                customer.DishOfOrderEatenButOrderNotFinished();
            }
        }
    }
}