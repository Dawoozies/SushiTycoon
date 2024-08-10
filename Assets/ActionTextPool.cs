using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class ActionTextPool : MonoBehaviour
{
    public static ActionTextPool ins;
    private void Awake()
    {
        ins = this;
    }
    [SerializeField] GameObject prefab;
    private void Start()
    {
        SharedGameObjectPool.Prewarm(prefab, 20);
    }
    public void Request(Func<ActionTextArgs> fetchFunc)
    {
        GameObject poolObj = SharedGameObjectPool.Rent(prefab);
        poolObj.transform.SetParent(transform, false);
        poolObj.transform.localScale = Vector3.one;
        ActionText instancedMonoBehaviour;
        if (poolObj.TryGetComponent(out instancedMonoBehaviour))
        {
            instancedMonoBehaviour.SetArgsFetch(fetchFunc);
        }
    }
}
