using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Diver : MonoBehaviour
{
    [SerializeField] DiverTask currentTask;
    [SerializeField] float oxygenCapacity;
    float oxygen;
    [SerializeField] float weightCapacity;
    float weight;
    [SerializeField] Transform collectionParent;
    [SerializeField] CircleCollider2D collectableDetectionCircle;
    [SerializeField] float collectableDetectMaxRadius;
    [SerializeField] float collectableDetectRadiusIncreaseRate;
    [SerializeField] float collectionRadius;
    [SerializeField] float collectionSpeed;
    [SerializeField] TriggerVolumeEvents targetDetectionEvents;
    DiverNavigation diverNavigationSystem;
    public UnityEvent<ICollectable> onCollectedCollectable;
    ICollectable targetedCollectable;

    [SerializeField] TriggerVolumeEvents resurfaceDetectionEvents;
    [SerializeField] bool atSurface;
    protected virtual void Start()
    {
        diverNavigationSystem = GetComponent<DiverNavigation>();
        targetDetectionEvents.RegisterCollisionCallback(OnTargetDetected, CollisionEventType.Enter);
        resurfaceDetectionEvents.RegisterCollisionCallback(OnSurfaceDetected, CollisionEventType.Stay);
        resurfaceDetectionEvents.RegisterCollisionCallback(OnSurfaceExit, CollisionEventType.Exit);
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
            if(!targetedCollectable.collected && Vector2.Distance(transform.position, targetedCollectable.position) <= collectionRadius)
            {
                targetedCollectable.Collect(collectionSpeed);
            }

            if(targetedCollectable.collected)
            {
                targetedCollectable.graphic.parent = collectionParent;
                diverNavigationSystem.ClearTarget();
                targetedCollectable = null;
            }
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
    }
    void OnTargetDetected(Collider2D col)
    {
        if (targetedCollectable != null)
            return;
        diverNavigationSystem.SetTarget(col.transform);
        targetedCollectable = col.transform.GetComponentInParent<ICollectable>();
    }
    void OnSurfaceDetected(Collider2D col)
    {
        atSurface = true;

        if(oxygen < oxygenCapacity)
        {
            oxygen += Time.deltaTime * 100f;
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
}
[Serializable]
public enum DiverTask
{
    Collect = 0,
    Resurface = 1,
}