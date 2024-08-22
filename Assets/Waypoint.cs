using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public void OnBuild()
    {
        WaitingArea.ins.OnObjectBuildHandler(gameObject);
    }
    public void OnRemove()
    {
        WaitingArea.ins.OnObjectDeletedHandler(gameObject);
    }
}
