using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class DockWorker : MonoBehaviour
{
    ICollectable heldCollectable;
    bool holdingSomething;
    public DockWorkerTask currentTask;
    [SerializeField] Transform holdPoint;
    [SerializeField, ReorderableList] GameObject[] arms;

    Navigation navigation;
    Boat boat;
    bool boatDocked;

    [SerializeField] TriggerVolumeEvents detectionEvents;
    private void Start()
    {
        navigation = GetComponent<Navigation>();
        detectionEvents.RegisterCollisionCallback(AtBoat, CollisionEventType.Stay);
        detectionEvents.RegisterCollisionCallback(AtItemDropOff, CollisionEventType.Stay);
    }
    private void Update()
    {
        SetActiveArms(holdingSomething ? 1 : 0);
        if(currentTask == DockWorkerTask.Idle && boatDocked)
        {
            currentTask = DockWorkerTask.Boat;
            navigation.SetTarget(boat.transform);
        }
        if(!boatDocked)
        {
            navigation.ClearTarget();
        }
        navigation.SetActiveNavigator((int)currentTask);
    }
    void SetActiveArms(int activeIndex)
    {
        for (int i = 0; i < arms.Length; i++)
        {
            arms[i].SetActive(i == activeIndex);
        }
    }
    public void BoatEnterDock(Boat boat)
    {
        this.boat = boat;
        boatDocked = true;
    }
    public void BoatExitDock()
    {
        boat = null;
        boatDocked = false;
    }
    void AtBoat(Collider2D other)
    {
        if(other.CompareTag("Boat"))
        {
            if (boatDocked && currentTask == DockWorkerTask.Boat)
            {
                heldCollectable = boat.TakeCollectableFromBoat();
                if (heldCollectable != null)
                {
                    heldCollectable.DockWorkerCollect(holdPoint);
                    holdingSomething = true;
                    currentTask = DockWorkerTask.ItemDropOff;
                }
            }
        }

    }
    void AtItemDropOff(Collider2D other)
    {
        if(other.CompareTag("ItemDropOff"))
        {
            if (holdingSomething)
            {
                IngredientStorage.ins.AddToStorage(heldCollectable.collectableData, 1);
                heldCollectable.ReturnToPool();
                heldCollectable = null;
                holdingSomething = false;
                currentTask = DockWorkerTask.Idle;
            }
        }
    }
}
public enum DockWorkerTask
{
    Idle,
    Boat,
    ItemDropOff,
}