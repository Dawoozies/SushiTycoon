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
    void Start()
    {
        SharedGameObjectPool.Prewarm(prefab, maxCustomers);
    }
    void Update()
    {
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
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        CustomerSpawnPoint spawnPoint = spawnPoints[spawnPointIndex];
        GameObject poolObj = SharedGameObjectPool.Rent(prefab);

        Customer customer = poolObj.GetComponent<Customer>();
        if(customer != null)
        {
            customer.OnSpawn(this, spawnPoint.RandomPos(), spawnPoint.movementDirection);
            customersActive++;
        }
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
            Gizmos.DrawWireSphere(t.spawnPoint.position, t.radius);
        }
    }
}