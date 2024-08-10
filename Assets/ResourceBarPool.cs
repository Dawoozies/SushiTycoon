using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;
public class ResourceBarPool : MonoBehaviour
{
    public static ResourceBarPool ins;
    private void Awake()
    {
        ins = this;
    }
    [SerializeField] GameObject prefab;
    private void Start()
    {
        SharedGameObjectPool.Prewarm(prefab, 20);
    }
    public void Request(Func<ResourceBarArgs> fetchFunc)
    {
        GameObject poolObj = SharedGameObjectPool.Rent(prefab);
        poolObj.transform.SetParent(transform, false);
        poolObj.transform.localScale = Vector3.one;
        ResourceBar instancedMonoBehaviour;
        if(poolObj.TryGetComponent(out instancedMonoBehaviour))
        {
            instancedMonoBehaviour.SetArgsFetch(fetchFunc);
        }
    }
}
