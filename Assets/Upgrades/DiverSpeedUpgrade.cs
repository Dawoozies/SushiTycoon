using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverSpeedUpgrade : MonoBehaviour
{

    public float diverSpeedIncrease = 0.2f;
    public void OnUpgrade()
    {
        RestaurantParameters.ins.DiverRunningSpeed += diverSpeedIncrease;
    }

}
