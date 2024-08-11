using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class TemporaryRigidbodyPool : MonoBehaviour
{
    public static TemporaryRigidbodyPool ins;
    private void Awake()
    {
        ins = this;
    }
    [SerializeField] GameObject prefab;
    private void Start()
    {
        SharedGameObjectPool.Prewarm(prefab, 20);
    }
    public void Request(Transform objToGiveRigidbody, out Transform rigidBodyTransform)
    {
        GameObject poolObj = SharedGameObjectPool.Rent(prefab);
        objToGiveRigidbody.SetParent(poolObj.transform, false);
        objToGiveRigidbody.localPosition = Vector3.zero;
        rigidBodyTransform = poolObj.transform;
    }
    public void Return(GameObject insToReturn)
    {
        SharedGameObjectPool.Return(insToReturn);
    }
}
