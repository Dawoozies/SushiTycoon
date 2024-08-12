using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;
public class CollectablePool : ObjectPoolBase<Collectable>
{
    GameObject collectablePrefab;
    public CollectablePool(GameObject collectablePrefab)
    {
        this.collectablePrefab = collectablePrefab;
    }
    protected override Collectable CreateInstance()
    {
        GameObject instancedObject = SharedGameObjectPool.Rent(collectablePrefab);
        Collectable instance = instancedObject.GetComponent<Collectable>();
        return instance;
    }
    protected override void OnDestroy(Collectable instance)
    {
    }
    protected override void OnRent(Collectable instance)
    {
    }
    protected override void OnReturn(Collectable instance)
    {
        SharedGameObjectPool.Return(instance.gameObject);
    }
}