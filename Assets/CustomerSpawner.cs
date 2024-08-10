using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : Spawner
{
    protected override void Start()
    {
        for (int spawn = 0; spawn < spawnPoints.Length; spawn++)
        {
            for (int i = 0; i < entitiesPerPoint; ++i)
            {
                Vector2 p = (Vector2)spawnPoints[spawn].position + Random.insideUnitCircle * spawnRadius;
                GameObject e = Instantiate(entityPrefab, p, Quaternion.identity);
                CustomerNavigationSystem navSystem = e.GetComponent<CustomerNavigationSystem>();
                if (spawn == 0)
                {
                    navSystem.SetMoveDirection(Vector3.right);
                }
                else
                {
                    navSystem.SetMoveDirection(Vector3.left);
                }
            }
        }
    }
}
