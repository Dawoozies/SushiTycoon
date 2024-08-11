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

    [SerializeField] TriggerVolumeEvents boatDetectionEvents;
    private void Start()
    {
        navigation = GetComponent<Navigation>();
    }
    private void Update()
    {
        SetActiveArms(holdingSomething ? 1 : 0);
        if(currentTask == DockWorkerTask.Idle && boatDocked)
        {
            currentTask = DockWorkerTask.Boat;
            navigation.SetTarget(boat.transform);
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
    public void OnBoatDetected(Collider2D boatCollider)
    {

    }
}
public enum DockWorkerTask
{
    Idle,
    Boat,
    ItemDropOff,
}