using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
    TargetNavigator targetNavigator;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        targetNavigator = GetComponent<TargetNavigator>();
        targetNavigator.MovementAllowed(true);
    }

    // Update is called once per frame
    void Update()
    {
        targetNavigator.SetTarget(target);
        targetNavigator.SetActiveNavigator(true);
        targetNavigator.Navigate();
    }
}
