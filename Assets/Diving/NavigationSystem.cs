using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NavigationSystem : MonoBehaviour
{
    TargetNavigator targetNavigator;
    INavigator[] taskNavigators;
    int currentNavigator;
    protected virtual void Start()
    {
        targetNavigator = GetComponent<TargetNavigator>();
        taskNavigators = GetComponents<INavigator>();
        foreach (INavigator navigator in taskNavigators)
        {
            navigator.MovementAllowed(true);
        }
    }
    protected virtual void Update()
    {
        for (int i = 0; i < taskNavigators.Length; i++)
        {
            taskNavigators[i].SetActiveNavigator(i == currentNavigator);
        }
        taskNavigators[currentNavigator].Navigate();
    }
    public virtual void SetTarget(Transform target)
    {
        targetNavigator.SetTarget(target);
    }
    public virtual void SetActiveNavigator(int index)
    {
        if (index >= taskNavigators.Length || index < 0)
            return;
        currentNavigator = index;
    }
    public virtual void ClearTarget()
    {
        targetNavigator.ClearTarget();
    }
}