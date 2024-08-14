using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class RestaurantParameters : MonoBehaviour
{
    public static RestaurantParameters ins;

    private void Awake()
    {
        ins = this;
    }

    public float SeatingDistance;
    public Vector2 PatienceBounds;
    public List<DishData> Menu;
    public float CustomerSatisfactionDecay = 0.004f;
    
    
    public DishData GetRandomMenuItem()
    {
        return Menu[Random.Range(0, Menu.Count)];
    }
}
