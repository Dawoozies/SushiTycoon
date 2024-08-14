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
    
    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {

    }
}
