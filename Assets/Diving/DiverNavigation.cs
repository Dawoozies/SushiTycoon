using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverNavigation : NavigationSystem
{
    [SerializeField] float oxygen;
    [SerializeField] float storageCapacityLeft;
    [SerializeField] TriggerVolumeEvents ingredientTriggerVolumeEvents;
    [SerializeField] DiverTask currentTask;
    TargetNavigator targetNavigator;
    INavigator[] taskNavigators;

    public Transform collectedIngredients;
    float collectionProgress;
    [SerializeField] float collectionDistance;
    Ingredient currentTargetedIngredient;
    protected override void Start()
    {
        ingredientTriggerVolumeEvents.RegisterCollisionCallback(OnIngredientDetectionEnter, CollisionEventType.Enter);
        targetNavigator = GetComponent<TargetNavigator>();
        taskNavigators = GetComponents<INavigator>();
        foreach (INavigator navigator in taskNavigators)
        {
            navigator.MovementAllowed(true);
        }
    }
    void OnIngredientDetectionEnter(Collider2D ingredientCol)
    {
        if(!targetNavigator.hasTarget)
        {
            targetNavigator.SetTarget(ingredientCol.transform);
            currentTargetedIngredient = ingredientCol.transform.GetComponentInParent<Ingredient>();
        }
    }
    protected override void Update()
    {
        if(oxygen > 0 && storageCapacityLeft > 0)
        {
            currentTask = DiverTask.GetIngredient;
        }
        else
        {
            currentTask = DiverTask.Resurface;
        }
        int navigator = (int)currentTask;
        for (int i = 0; i < taskNavigators.Length; i++)
        {
            taskNavigators[i].SetActiveNavigator(i == navigator);
        }
        taskNavigators[navigator].Navigate();

        if(currentTask == DiverTask.GetIngredient)
        {
            if(currentTargetedIngredient != null)
            {
                float d = targetNavigator.DistanceFromTarget();
                if (d >= 0 && d < collectionDistance)
                {
                    currentTargetedIngredient.collectionProgress += Time.deltaTime;
                }

                if(currentTargetedIngredient.collectionProgress > currentTargetedIngredient.collectionTime)
                {
                    currentTargetedIngredient.collected = true;
                    currentTargetedIngredient.transform.parent = collectedIngredients;
                    currentTargetedIngredient.gameObject.SetActive(false);
                    currentTargetedIngredient = null;
                    targetNavigator.ClearTarget();
                }
            }
        }
    }
}
[Serializable]
public enum DiverTask
{
    GetIngredient = 0,
    Resurface = 1,
}