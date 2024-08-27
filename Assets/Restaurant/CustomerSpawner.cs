using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using uPools;
public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField, ReorderableList] CustomerSpawnPoint[] spawnPoints;
    private float _spawnSideBias;
    [SerializeField] float spawnBiasDelta;
    [Range(0f, 1f)]
    public float spawnSideBias;
[Serializable]
    public class CustomerSpawnPoint
    {
        public Transform spawnPoint;
        public float radius;
        public Vector2 movementDirection;
        public Vector2 RandomPos()
        {
            return (Vector2)spawnPoint.position + Random.insideUnitCircle*radius;
        }
    }
    [SerializeField] int maxCustomers;
    [SerializeField, Disable] int customersActive;
    [SerializeField] float spawnTime;
    float _spawnTime;
    [SerializeField] AnimationCurve spawnProbabilityCurve;
    [SerializeField, Disable] float currentSpawnChance;
    public Transform[] despawners;
    public bool allowSpawning;
    float openTime;
    void Start()
    {
        SharedGameObjectPool.Prewarm(prefab, maxCustomers);
        spawnSideBias = 0.5f;
    }
    void Update()
    {
        if(RestaurantParameters.ins.RestaurantOpen)
        {
            allowSpawning = true;
        }
        else
        {
            allowSpawning = false;
        }

        if (!allowSpawning)
            return;

        float activePercentage = Mathf.Clamp01((float)customersActive / (float)maxCustomers);
        currentSpawnChance = spawnProbabilityCurve.Evaluate(activePercentage);
        if(_spawnTime > 0)
        {
            _spawnTime -= Time.deltaTime;
        }
        else
        {
            _spawnTime = spawnTime;
            TrySpawn();
        }
        SpawnBiasUpdater();
    }
    void TrySpawn()
    {
        float spawnRoll = Random.Range(0f, 1f);
        bool rollSuccess = spawnRoll < currentSpawnChance;
        if (!rollSuccess)
            return;
        Spawn();
    }
    void Spawn()
    {
        if (customersActive >= maxCustomers)
            return;
        //OLD CODE BEFORE BIAS ADDED (used for more than 2 spawn points) // int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        int spawnPointIndex;
        if (SpawnBiasLeftSide()) spawnPointIndex = 0;
        else spawnPointIndex = 1;

        CustomerSpawnPoint spawnPoint = spawnPoints[spawnPointIndex];
        GameObject poolObj = SharedGameObjectPool.Rent(prefab);

        Customer customer = poolObj.GetComponent<Customer>();
        if(customer != null)
        {
            customer.OnSpawn(this, spawnPoint.RandomPos(), spawnPoint.movementDirection);
            customersActive++;
        }
    }
    bool SpawnBiasLeftSide()
    {
        // To encourage crowd grouping/waves
        if (spawnSideBias < 0.2f) return true;
        if (spawnSideBias > 0.8f) return false;

        if (spawnSideBias < Random.value) return true;
        else return false;
    }
    void SpawnBiasUpdater()
    {
        if (Random.Range(0f, 1f) >= 0.5f)
        {
            spawnSideBias += spawnBiasDelta * Time.deltaTime;
        }
        else
        {
            spawnSideBias -= spawnBiasDelta * Time.deltaTime;
        }
        spawnSideBias = Mathf.Clamp01(spawnSideBias);
        if (spawnSideBias < 0f || spawnSideBias > 1f) Debug.Log("Invalid Spawn Bias: " + spawnSideBias);

    }
    public void Return(GameObject poolInstance)
    {
        SharedGameObjectPool.Return(poolInstance);
        customersActive--;
    }
    public Vector2 DespawnerPositionRandom()
    {
        int despawnerIndex = Random.Range(0, despawners.Length);
        return despawners[despawnerIndex].position;
    }
    void OnDrawGizmosSelected()
    {
        foreach (var t in spawnPoints)
        {
            Gizmos.color = Color.cyan;
            if (t.spawnPoint == null)
                continue;
            Gizmos.DrawWireSphere(t.spawnPoint.position, t.radius);
        }
    }
}