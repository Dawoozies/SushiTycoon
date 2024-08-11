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
    public bool atDestination;
    Vector2 p;
    [SerializeField] Transform holdPoint;
    [SerializeField] float stoppingDistance;
    [SerializeField] float moveSpeed;
    Rigidbody2D rb;
    Vector2 dirToTarget { 
        get {
            Vector2 d = p  - (Vector2)transform.position;
            d.y = 0f;
            return d.normalized;
        }
    }
    [SerializeField, ReorderableList] GameObject[] arms;
    Boat boat;
    bool boatDocked;

    Transform[] dockPoints;
    Vector2 idleWalkPos;
    [SerializeField] float idleWalkTime;
    float _idleWalkTime;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void SetUpDockWorkerPoints(Transform[] dockPoints)
    {
        this.dockPoints = dockPoints;
    }
    private void FixedUpdate()
    {
        Vector2 dv = dirToTarget * moveSpeed * Time.fixedDeltaTime;
        Vector2 nextPos = rb.position + dv;
        if(!atDestination)
        {
            rb.MovePosition(nextPos);
        }
    }
    private void Update()
    {
        if(currentTask == DockWorkerTask.Idle)
        {
            if (_idleWalkTime > 0)
            {
                _idleWalkTime -= Time.deltaTime;
            }
            else
            {
                _idleWalkTime = idleWalkTime;
                idleWalkPos = Random.insideUnitCircle;
            }
        }

        if(currentTask == DockWorkerTask.Idle && boatDocked)
        {
            currentTask = DockWorkerTask.Boat;
        }

        if (holdingSomething)
        {
            SetActiveArms(1);
        }
        else
        {
            SetActiveArms(0);
        }

        p = dockPoints[(int)currentTask].position;
        if(currentTask == DockWorkerTask.Idle)
        {
            p += idleWalkPos;
        }

        atDestination = Vector2.Distance(transform.position, p) < stoppingDistance;

        if(currentTask == DockWorkerTask.Boat && atDestination)
        {
            heldCollectable = boat.TakeCollectableFromBoat();
            if(heldCollectable != null)
            {
                heldCollectable.DockWorkerCollect(holdPoint);
                holdingSomething = true;
                currentTask = DockWorkerTask.ItemDropOff;
            }
        }

        if(currentTask == DockWorkerTask.ItemDropOff && atDestination)
        {
            IngredientStorage.ins.AddToStorage(heldCollectable.collectableData, 1);
            heldCollectable.ReturnToPool();
            heldCollectable = null;
            holdingSomething = false;
            currentTask = DockWorkerTask.Idle;
        }
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
    public void BoatExitDock(Boat boat)
    {
        if (this.boat = boat)
        {
            this.boat = null;
            boatDocked = false;
        }
    }
}
public enum DockWorkerTask
{
    Idle,
    Boat,
    ItemDropOff,
}