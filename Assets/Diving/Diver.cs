using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Diver : MonoBehaviour
{
    [SerializeField] DiverTask currentTask;
    [SerializeField, BeginGroup("Oxygen Parameters", Style = GroupStyle.Round)] 
    float oxygenCapacity;
    [SerializeField] float oxygen;
    [SerializeField] float oxygenRestoreRate;
    ResourceBarArgs oxygenResourceBarArgs = new();
    [SerializeField] Transform oxygenBarPlacement;
    [SerializeField, ReorderableList, EndGroup] Gradient[] oxygenBarGradients;

    [SerializeField, BeginGroup("Weight Parameters", Style = GroupStyle.Round)] 
    float weightCapacity;
    [SerializeField] float weight;
    ResourceBarArgs weightResourceBarArgs = new();
    [SerializeField] Transform weightBarPlacement;
    [SerializeField, ReorderableList, EndGroup] Gradient[] weightBarGradients;

    [SerializeField, BeginGroup("Collecting Parameters", Style = GroupStyle.Round)] Transform collectionParent;
    [SerializeField] CircleCollider2D collectableDetectionCircle;
    [SerializeField] float collectableDetectMaxRadius;
    [SerializeField] float collectableDetectRadiusIncreaseRate;
    [SerializeField] float collectionRadius;
    [SerializeField] float collectionSpeed;
    [SerializeField] TriggerVolumeEvents targetDetectionEvents;
    DiverNavigation diverNavigationSystem;
    [SerializeField] Transform collectionTextPlacement;
    [EndGroup] public UnityEvent<ICollectable> onCollectedCollectable;
    ICollectable targetedCollectable;
    ActionTextArgs collectionTextArgs = new();
    bool inCollectionRadius;
    List<ICollectable> bag = new();

    [SerializeField, BeginGroup("Resurface Parameters", Style = GroupStyle.Round)] 
    TriggerVolumeEvents resurfaceDetectionEvents;
    [SerializeField, EndGroup] bool atSurface;
    protected virtual void Start()
    {
        diverNavigationSystem = GetComponent<DiverNavigation>();
        targetDetectionEvents.RegisterCollisionCallback(OnTargetDetected, CollisionEventType.Enter);
        resurfaceDetectionEvents.RegisterCollisionCallback(OnSurfaceDetected, CollisionEventType.Stay);
        resurfaceDetectionEvents.RegisterCollisionCallback(OnSurfaceExit, CollisionEventType.Exit);

        ResourceBarPool.ins.Request(OxygenBarFetch);
        ResourceBarPool.ins.Request(WeightBarFetch);
        ActionTextPool.ins.Request(CollectionTextFetch);
    }
    protected virtual void Update()
    {
        collectableDetectionCircle.radius += collectableDetectRadiusIncreaseRate * Time.deltaTime;
        if(collectableDetectionCircle.radius > collectableDetectMaxRadius)
        {
            collectableDetectionCircle.radius = 0.01f;
        }

        if(targetedCollectable != null)
        {
            inCollectionRadius = Vector2.Distance(transform.position, targetedCollectable.position) <= collectionRadius;
            if (inCollectionRadius)
            {
                if (!targetedCollectable.collected)
                {
                    targetedCollectable.CollectProgress(collectionSpeed);
                }
                if (targetedCollectable.collected)
                {
                    targetedCollectable.Collect(collectionParent, ref bag);
                    diverNavigationSystem.ClearTarget();
                    targetedCollectable = null;
                }
            }
            else
            {
                if(targetedCollectable.collected)
                {
                    diverNavigationSystem.ClearTarget();
                    targetedCollectable = null;
                }
            }
        }
        else
        {
            inCollectionRadius = false;
        }

        if(currentTask != DiverTask.Resurface && oxygen > 0 && weight < weightCapacity)
        {
            currentTask = DiverTask.Collect;
        }
        if(oxygen <= 0 || weight >= weightCapacity)
        {
            currentTask = DiverTask.Resurface;
        }

        diverNavigationSystem.SetActiveNavigator((int)currentTask);

        if(!atSurface && oxygen > 0)
        {
            oxygen -= Time.deltaTime;
        }

        CalculateWeight();
    }
    void CalculateWeight()
    {
        weight = 0f;
        foreach (var item in bag)
        {
            weight += item.weight;
        }
    }
    void OnTargetDetected(Collider2D col)
    {
        if (targetedCollectable != null)
            return;
        ICollectable detectedTarget = col.transform.GetComponentInParent<ICollectable>();
        if(detectedTarget == null)
            return;
        if (detectedTarget.collected)
            return;
        targetedCollectable = detectedTarget;
        diverNavigationSystem.SetTarget(col.transform);
    }
    void OnSurfaceDetected(Collider2D col)
    {
        atSurface = true;

        if(oxygen < oxygenCapacity)
        {
            oxygen += Time.deltaTime * oxygenRestoreRate;
            if(oxygen > oxygenCapacity)
            {
                oxygen = oxygenCapacity;
            }
        }

        if(currentTask == DiverTask.Resurface && oxygen >= oxygenCapacity)
        {
            currentTask = DiverTask.Collect;
        }
    }
    void OnSurfaceExit(Collider2D col)
    {
        atSurface = false;
    }
    ResourceBarArgs OxygenBarFetch()
    {
        oxygenResourceBarArgs.worldPos = oxygenBarPlacement.position;
        oxygenResourceBarArgs.maxValue = oxygenCapacity;
        oxygenResourceBarArgs.value = oxygen;
        oxygenResourceBarArgs.gradients = oxygenBarGradients;
        return oxygenResourceBarArgs;
    }
    ResourceBarArgs WeightBarFetch()
    {
        weightResourceBarArgs.worldPos = weightBarPlacement.position;
        weightResourceBarArgs.maxValue = weightCapacity;
        weightResourceBarArgs.value = weight;
        weightResourceBarArgs.gradients = weightBarGradients;
        return weightResourceBarArgs;
    }
    ActionTextArgs CollectionTextFetch()
    {
        collectionTextArgs.worldPos = collectionTextPlacement.position;
        string[] textLines = { "", "" };
        if (currentTask == DiverTask.Collect && inCollectionRadius && targetedCollectable != null)
        {
            textLines[0] = "Collecting";
            textLines[1] = $"{Mathf.Floor(targetedCollectable.progressPercentage*10)/10}%";

        }
        collectionTextArgs.textLines = textLines;
        return collectionTextArgs;
    }
    public ICollectable TakeCollectableFromBag()
    {
        if(bag.Count == 0)
        {
            return null;
        }

        ICollectable collectable = bag[0];
        bag.RemoveAt(0);
        return collectable;
    }
}
[Serializable]
public enum DiverTask
{
    Collect = 0,
    Resurface = 1,
}