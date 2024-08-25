using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class Dock : MonoBehaviour
{
    [SerializeField] GameObject dockWorkerPrefab;
    [SerializeField] Transform dockSpawnPoint;
    bool boatIsDocked;
    Boat dockedBoat;

    [SerializeField] float workerAssignTime;
    float _workerAssignTime;
    public float hiringCost;
    [SerializeField, ReorderableList] Transform[] dockPoints;
    public int workerCount;
    [SerializeField, ReorderableList] List<DockWorker> workers = new();
    private void Start()
    {
        SharedGameObjectPool.Prewarm(dockWorkerPrefab, 20);
        for (int i = 0; i < workerCount; i++) 
        {
            AddNewDockWorker();
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log($"{other.transform.name} enter {gameObject.name}");
        if (other.CompareTag("Boat"))
        {
            if(dockedBoat == null)
                dockedBoat = other.transform.GetComponent<Boat>();
            foreach (DockWorker worker in workers)
            {
                worker.BoatEnterDock(dockedBoat);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log($"{other.transform.name} exited {gameObject.name}");
        if (other.CompareTag("Boat"))
        {
            foreach (DockWorker worker in workers)
            {
                worker.BoatExitDock();
            }

            dockedBoat = null;
            boatIsDocked = false;
        }
    }
    private void Update()
    {
        if(boatIsDocked)
        {
            dockPoints[1].position = dockedBoat.transform.position;
        }
    }
    void AddNewDockWorker()
    {
        GameObject dockWorkerPoolObj = SharedGameObjectPool.Rent(dockWorkerPrefab);
        DockWorker dockWorker = dockWorkerPoolObj.GetComponent<DockWorker>();
        dockWorker.transform.parent = transform;
        dockWorker.transform.position = dockSpawnPoint.position + Vector3.right * Random.Range(-0.4f, 0.2f);
        workers.Add(dockWorker);
    }
    public void Hire()
    {
        if(RestaurantParameters.ins.TryBuyItem(hiringCost))
        {
            AddNewDockWorker();
        }
    }
}
