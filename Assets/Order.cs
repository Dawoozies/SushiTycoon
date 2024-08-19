using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using uPools;
public class Order : MonoBehaviour
{
    [SerializeField, Disable] Waiter assignedWaiter;

    [SerializeField, Disable] List<DishData> order = new();
    [SerializeField, Disable] Customer customer;
    public UnityEvent onOrderCreated;
    public UnityEvent onWaiterPickUp;

    //while all
    List<Chef> assignedChefs = new();
    List<Dish> completedDishes = new();
    List<DishData> unassignedDishes = new();
    //chefs all look at orders, assign themselves to completing a single dish
    //The serving counter which is storing this order
    ServingCounter assignedServingCounter;
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
        unassignedDishes = order;
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
    public void AssignedWaiterPickUp(Transform holdPoint)
    {
        transform.parent = holdPoint;
        transform.localPosition = Vector3.zero;
        onWaiterPickUp?.Invoke();
    }
    public void AssignServingCounter(ServingCounter servingCounter)
    {
        assignedServingCounter = servingCounter;
    }
    //a chef assigned to a dish in an order once completing the order will check if there is any other stage of prep to complete
    public bool TryAssignChefToDish(Chef assignedChef, ref Dish dish, ref Order order, ref Action<Chef, Dish> dishCompleteCallback)
    {
        //all dishes have been assigned to chefs
        if (unassignedDishes.Count == 0)
            return false;
        if(completedDishes.Count >= this.order.Count)
        //then chef already assigned to this order in some way so should not attempt another dish prep
        if (assignedChefs.Contains(assignedChef))
            return false;
        //when dish complete the chef that is assigned to the order should clear themselves if there is no more work to do

        assignedChefs.Add(assignedChef);
        dish = SharedGameObjectPool.Rent(RestaurantParameters.ins.DishPrefab).GetComponent<Dish>();
        //initialize dish with the assigned chef
        Debug.Log(dish);
        Debug.Log(assignedChef);
        Debug.Log(unassignedDishes[0]);
        dish.InitializeDish(assignedChef, unassignedDishes[0]);
        unassignedDishes.RemoveAt(0);
        order = this;
        dishCompleteCallback = ChefDishCompletedHandler;
        return true;
    }
    void ChefDishCompletedHandler(Chef whoCompletedDish, Dish completedDish)
    {
        //when a dish has been completed update dishes left for order
        int oldDishesToCompleteValue = order.Count - completedDishes.Count;
        completedDishes.Add(completedDish);
        assignedChefs.Remove(whoCompletedDish);
        int newDishesToCompleteValue = order.Count - completedDishes.Count;
        assignedServingCounter.UpdateDishesLeftForOrder(this, oldDishesToCompleteValue, newDishesToCompleteValue);
    }
    public int GetDishesToComplete()
    {
        return order.Count - completedDishes.Count;
    }
}
