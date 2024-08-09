using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [ReorderableList]
    public Transform[] spawnPoints;
    [SerializeField] float spawnRadius;
    [SerializeField] int entitiesPerPoint;
    [SerializeField] GameObject entityPrefab;
    [SerializeField, ReorderableList] CollectableData[] collectableData;
    protected virtual void Start()
    {
        foreach (Transform t in spawnPoints)
        {
            for (int i = 0; i < entitiesPerPoint; ++i)
            {
                Vector2 p = (Vector2)t.position + Random.insideUnitCircle * spawnRadius;
                GameObject e = Instantiate(entityPrefab, p, Quaternion.identity);
                ICollectable collectable;
                if(e.TryGetComponent(out collectable))
                {
                    collectable.SetCollectableData(collectableData[Random.Range(0,collectableData.Length)]);
                }
            }
        }
    }
    protected virtual void OnDrawGizmosSelected()
    {
        foreach(Transform t in spawnPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(t.position, spawnRadius);
        }
    }
}
