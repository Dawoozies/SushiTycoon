using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class DockWorker : MonoBehaviour
{
    ICollectable heldCollectable;
    [SerializeField]bool holdingSomething;
    public DockWorkerTask currentTask;
    [SerializeField] Transform holdPoint;
    [SerializeField, ReorderableList] GameObject[] arms;

    Navigation navigation;
    Boat boat;
    bool boatDocked;

    [SerializeField] TriggerVolumeEvents boatDetectionEvents;
    [SerializeField] TriggerVolumeEvents itemDropOffDetectionEvents;
    private void Start()
    {
        navigation = GetComponent<Navigation>();
        boatDetectionEvents.RegisterCollisionCallback(AtBoat, CollisionEventType.Stay);
        itemDropOffDetectionEvents.RegisterCollisionCallback(AtItemDropOff, CollisionEventType.Stay);
    }
    private void Update()
    {
        SetActiveArms(holdingSomething ? 1 : 0);
        if(currentTask == DockWorkerTask.Idle && boatDocked && boat.CollectablesOnBoat() > 0)
        {
            currentTask = DockWorkerTask.Boat;
            navigation.SetTarget(boat.transform);
        }
        if(!holdingSomething && currentTask == DockWorkerTask.Boat && boatDocked && boat.CollectablesOnBoat() == 0)
        {
            currentTask = DockWorkerTask.Idle;
            navigation.ClearTarget();
        }
        if(!holdingSomething && currentTask == DockWorkerTask.Boat && !boatDocked)
        {
            currentTask = DockWorkerTask.Idle;
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
    void AtItemDropOff(Collider2D other)
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
public enum DockWorkerTask
{
    Idle,
    Boat,
    ItemDropOff,
}