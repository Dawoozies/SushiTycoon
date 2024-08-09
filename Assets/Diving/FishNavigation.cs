using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishNavigation : NavigationSystem
{
    [Serializable]
    public enum NavigationToUse
    {
        RandomWalk = 0,
        FleeFromOther = 1,
        MoveToOther = 2,
    }
    [SerializeField] NavigationToUse navigationToUse;
    INavigator[] navigators;
    protected override void Start()
    {
        base.Start();
        navigators = GetComponents<INavigator>();
        foreach (INavigator n in navigators)
        {
            n.MovementAllowed(true);
        }
    }
    protected override void Update()
    {
        base.Update();
        int navigator = (int)navigationToUse;
        for (int i = 0; i < navigators.Length; i++)
        {
            navigators[i].SetActiveNavigator(i == navigator);
        }
        navigators[navigator].Navigate();
    }
}