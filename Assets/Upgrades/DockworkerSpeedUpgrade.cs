using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockworkerSpeedUpgrade : MonoBehaviour
{

    public float dockworkerSpeedIncrease = 0.2f;
    public void OnUpgrade()
    {
        RestaurantParameters.ins.DockWorkerRunningSpeed += dockworkerSpeedIncrease;
    }

}
