using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private float _satisfaction;
    public float satisfaction { get => Mathf.Clamp01(_satisfaction); set => _satisfaction = Mathf.Clamp01(value); }
    public float satisfactionDecay; // Decays satisfaction by this amount per second
    public bool satisfactionDecayable;
    public float patience;      // Seconds customer is willing to wait
    private float patienceDefault; // Customer's default patience
    public List<DishData> order;

    CustomerNavigationSystem navSystem;
    float customerEnterRestaurantCheckTime;
    bool enteredRestaurant;
    void Start()
    {
        satisfaction = 0.5f;        // Starts at 50%
        satisfactionDecay = RestaurantParameters.ins.CustomerSatisfactionDecay; // Decays 24% per minute
        patienceDefault = Random.Range(RestaurantParameters.ins.PatienceBounds.x, RestaurantParameters.ins.PatienceBounds.y);
        patience = patienceDefault;
        satisfactionDecayable = false;
    }
    List<DishData> GetMenu(List<DishData> dishData)
    {
        return dishData;
    }
    public void ResetPatience()
    {
        patience = patienceDefault;
    }
    void Update()
    {
        if (enteredRestaurant)
            return;
        if(customerEnterRestaurantCheckTime > 0)
        {
            customerEnterRestaurantCheckTime -= Time.deltaTime;
        }
        else
        {
            customerEnterRestaurantCheckTime = RestaurantParameters.ins.CustomerEnterRestaurantCheckTime;
            RollEnterRestaurant();
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
}
