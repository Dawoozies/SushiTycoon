using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [ReorderableList]
    public Transform[] spawnPoints;
    [SerializeField] protected float spawnRadius;
    [SerializeField] protected int entitiesPerPoint;
    [SerializeField] protected GameObject entityPrefab;
    protected virtual void Start()
    {
        foreach (Transform t in spawnPoints)
        {
            for (int i = 0; i < entitiesPerPoint; ++i)
            {
                Vector2 p = (Vector2)t.position + Random.insideUnitCircle * spawnRadius;
                GameObject e = Instantiate(entityPrefab, p, Quaternion.identity);
            }
        }
    }
    protected virtual void OnDrawGizmosSelected()
    {
        foreach (Transform t in spawnPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(t.position, spawnRadius);
        }
    }
}
