using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterNavigator : Navigator
{

    public bool isIdle;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        isIdle = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
