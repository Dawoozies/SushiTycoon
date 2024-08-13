using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class CollectionNotificationPool : MonoBehaviour
{
    public static CollectionNotificationPool ins;
    void Awake()
    {
        ins = this;
    }
    public GameObject prefab;
    void Start()
    {
        SharedGameObjectPool.Prewarm(prefab, 10);
    }
    public void Request(CollectableData collectable)
    {
        GameObject poolObj = SharedGameObjectPool.Rent(prefab);
        poolObj.transform.SetParent(transform, false);
        CollectionNotification notification;
        if(poolObj.TryGetComponent(out notification))
        {
            notification.TriggerNotification(collectable);
            poolObj.transform.SetAsFirstSibling();
        }
    }
}
