using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using uPools;
using Random = UnityEngine.Random;
public class CollectableSpawner : MonoBehaviour
{
    [SerializeField]
    InitialSpawnPoint[] initialSpawnPoints;
    [SerializeField]
    SpawnPoint[] respawnPoints;
    public GameObject prefab;
    [SerializeField] float respawnTime;
    float _respawnTime;
    [SerializeField] AnimationCurve respawnProbabilityCurve;
    [SerializeField, Disable] float currentRespawnChance;
    [SerializeField, Disable] float inactivePercentage;
    [SerializeField, Disable] int totalCollectables;
    [SerializeField, Disable] int activeCollectables;
    [SerializeField, Disable] int inactiveCollectables;
    void Start()
    {
        InitialSpawn();
    }
    void InitialSpawn()
    {
        foreach(InitialSpawnPoint initialSpawnPoint in initialSpawnPoints)
        {
            for(int i = 0; i < initialSpawnPoint.initialSpawnCount; i++)
            {
                CollectableData collectableData = initialSpawnPoint.RandomCollectableData();
                GameObject collectableMainObject = SharedGameObjectPool.Rent(prefab);
                collectableMainObject.transform.position = initialSpawnPoint.RandomNavMeshPosition();
                Collectable collectable = collectableMainObject.GetComponent<Collectable>();
                collectable.SetCollectableData(collectableData);
                collectable.SetSpawnerReturnCallback(() => {
                    collectableMainObject.transform.parent = null;
                    SharedGameObjectPool.Return(collectableMainObject);
                    inactiveCollectables++;
                    activeCollectables--;
                });
                activeCollectables++;
                totalCollectables++;
            }
        }
    }
    private void Update()
    {
        inactivePercentage = Mathf.Clamp01(((float)inactiveCollectables/(float)totalCollectables));
        currentRespawnChance = respawnProbabilityCurve.Evaluate(inactivePercentage);

        if (_respawnTime > 0)
        {
            _respawnTime -= Time.deltaTime;
        }
        else
        {
            _respawnTime = respawnTime;
            TryRespawn();
        }
    }
    void TryRespawn()
    {
        float respawnRoll = Random.Range(0f,1f);
        bool rollSuccess = respawnRoll < currentRespawnChance;
        if (!rollSuccess)
            return;
        Respawn();
    }
    void Respawn()
    {
        int index = Random.Range(0, respawnPoints.Length);
        SpawnPoint respawnPoint = respawnPoints[index];
        CollectableData collectableData = respawnPoint.RandomCollectableData();
        GameObject collectableMainObject = SharedGameObjectPool.Rent(prefab);
        collectableMainObject.transform.position = respawnPoint.RandomNavMeshPosition();
        Collectable collectable = collectableMainObject.GetComponent<Collectable>();
        collectable.SetCollectableData(collectableData);
        collectable.SetSpawnerReturnCallback(() => {
            collectableMainObject.transform.parent = null;
            SharedGameObjectPool.Return(collectableMainObject);
            inactiveCollectables++;
            activeCollectables++;
        });
        activeCollectables++;
        inactiveCollectables--;
    }
    private void OnDrawGizmosSelected()
    {
        foreach (var t in initialSpawnPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(t.position, t.radius);
        }
        foreach (var t in respawnPoints)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(t.position, t.radius);
        }
    }
}
[Serializable]
public class SpawnPoint
{
    public Transform transform;
    public float radius;
    [SerializeField] CollectableData[] collectableData;
    public Vector3 position => transform.position;
    public Vector2 RandomPosition()
    {
        return (Vector2)transform.position + Random.insideUnitCircle * radius;
    }
    public Vector2 RandomNavMeshPosition()
    {
        Vector3 p = transform.position + Random.insideUnitSphere * radius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(p, out hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return p;
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
    public int initialSpawnCount;
}