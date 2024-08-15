using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class Customer : MonoBehaviour
{
    private float _satisfaction;
    public float satisfaction { get => Mathf.Clamp01(_satisfaction); set => _satisfaction = Mathf.Clamp01(value); }
    public float satisfactionDecay; // Decays satisfaction by this amount per second
    public bool satisfactionDecayable;
    public float patience;      // Seconds customer is willing to wait
    private float patienceDefault; // Customer's default patience

    CustomerNavigationSystem navSystem;
    float customerEnterRestaurantCheckTime;
    bool enteredRestaurant;


    bool hasOrdered;
    float orderTime;

    Waiter orderTakingWaiter;
    public bool readyToOrder;

    Waiter servingWaiter;
    void Start()
    {
        satisfaction = 0.5f;        // Starts at 50%
        satisfactionDecay = RestaurantParameters.ins.CustomerSatisfactionDecay; // Decays 24% per minute
        patienceDefault = Random.Range(RestaurantParameters.ins.PatienceBounds.x, RestaurantParameters.ins.PatienceBounds.y);
        patience = patienceDefault;
        satisfactionDecayable = false;
    }
    public void ResetPatience()
    {
        patience = patienceDefault;
    }
    void Update()
    {
        if (!enteredRestaurant)
        {
            if (customerEnterRestaurantCheckTime > 0)
            {
                customerEnterRestaurantCheckTime -= Time.deltaTime;
            }
            else
            {
                customerEnterRestaurantCheckTime = RestaurantParameters.ins.CustomerEnterRestaurantCheckTime;
                RollEnterRestaurant();
            }
        }
        if(navSystem.currentTask == CustomerTask.Seated)
        {
            if(!hasOrdered)
            {
                if(orderTime > 0)
                {
                    orderTime -= Time.deltaTime;
                }
                else
                {
                    readyToOrder = true;
                }
            }
        }
    }
    public void OnSpawn(CustomerSpawner spawner, Vector3 spawnPos, Vector3 moveDir)
    {
        satisfaction = 0.5f;
        patienceDefault = Random.Range(RestaurantParameters.ins.PatienceBounds.x, RestaurantParameters.ins.PatienceBounds.y);
        ResetPatience();
        satisfactionDecayable = false;

        if (navSystem == null)
            navSystem = GetComponent<CustomerNavigationSystem>();

        navSystem.SetMoveDirection(moveDir);
        navSystem.WarpAgent(spawnPos);
        navSystem.SetSpawnerReference(spawner);
        navSystem.InitialiseNavigator();

        customerEnterRestaurantCheckTime = RestaurantParameters.ins.CustomerEnterRestaurantCheckTime;
        enteredRestaurant = false;

        orderTime = RestaurantParameters.ins.GetRandomOrderingTime();
        readyToOrder = false;
    }
    void RollEnterRestaurant()
    {
        float roll = Random.Range(0f, 1f);
        bool rollSuccess = roll < RestaurantParameters.ins.CustomerEnterRestaurantChance;
        if (!rollSuccess)
            return;
        navSystem.SetTask(CustomerTask.Queueing);
        enteredRestaurant = true;
    }
    public bool TryAssignOrderTakingWaiter(Waiter waiter)
    {
        if (orderTakingWaiter != null && orderTakingWaiter != waiter)
            return false;
        orderTakingWaiter = waiter;
        return true;
    }
    public bool TryTakeOrder(Waiter waiter, out Order order)
    {
        order = null;
        if (orderTakingWaiter != waiter)
            return false;

        if(Vector2.Distance(waiter.transform.position, transform.position) > RestaurantParameters.ins.ValidOrderTakingDistance)
        {
            return false;
        }

        order = SharedGameObjectPool.Rent(RestaurantParameters.ins.OrderPrefab).GetComponent<Order>();
        order.CustomerCreateOrder(this, RestaurantParameters.ins.GetRandomMenuItem());
        hasOrdered = true;

        orderTakingWaiter = null;
        readyToOrder = false;

        return true;
    }
}
