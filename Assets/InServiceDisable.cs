using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InServiceDisable : MonoBehaviour
{
    public UnityEvent onInService;
    public UnityEvent onNotInService;
    bool _inService;
    void Update()
    {
        if(_inService != RestaurantParameters.ins.InService)
        {
            _inService = RestaurantParameters.ins.InService;
            if (_inService)
            {
                onInService?.Invoke();
            }
            else
            {
                onNotInService?.Invoke();
            }
        }
    }
}
