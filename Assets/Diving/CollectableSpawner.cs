using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class CollectableSpawner : MonoBehaviour
{
    [SerializeField, ReorderableList]
    InitialSpawnPoint[] initialSpawnPoints;
    [SerializeField, ReorderableList]
    SpawnPoint[] respawnPoints;
    CollectablePool pool;
    void Start()
    {
        pool.Prewarm(20);
        InitialSpawn();
    }
    void InitialSpawn()
    {
        foreach(InitialSpawnPoint initialSpawnPoint in initialSpawnPoints)
        {
            Collectable instance = pool.Rent();
            instance.SetCollectableData(initialSpawnPoint.RandomCollectableData());
        }
    }
}
[Serializable]
public class SpawnPoint
{
    [SerializeField] Transform transform;
    [SerializeField] float radius;
    [SerializeField] CollectableData[] collectableData;
    public Vector2 RandomPosition()
    {
        return (Vector2)transform.position + Random.insideUnitCircle * radius;
    }
    public CollectableData RandomCollectableData()
    {
        int index = Random.Range(0, collectableData.Length);
        return collectableData[index];
    }
}
[Serializable]
public class InitialSpawnPoint : SpawnPoint
{
    [SerializeField] int initialSpawnCount;
}