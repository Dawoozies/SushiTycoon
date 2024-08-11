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
    [SerializeField, ReorderableList] Transform[] dockPoints;
    [SerializeField, ReorderableList] List<DockWorker> workers = new();

    private void Start()
    {
        SharedGameObjectPool.Prewarm(dockWorkerPrefab, 20);
        for (int i = 0; i < 3; i++) 
        {
            AddNewDockWorker();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{other.transform.name} entered {gameObject.name}");
        if (other.CompareTag("Boat"))
        {
            dockedBoat = other.transform.GetComponent<Boat>();
            boatIsDocked = true;

            foreach (DockWorker worker in workers)
            {
                worker.BoatEnterDock(dockedBoat);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"{other.transform.name} exited {gameObject.name}");
        if (other.CompareTag("Boat"))
        {
            dockedBoat = null;
            boatIsDocked = false;

            foreach (DockWorker worker in workers)
            {
                worker.BoatExitDock(dockedBoat);
            }
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
        dockWorker.SetUpDockWorkerPoints(dockPoints);
        workers.Add(dockWorker);
    }
}
