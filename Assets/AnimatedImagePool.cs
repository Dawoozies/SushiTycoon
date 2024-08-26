using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class AnimatedImagePool : MonoBehaviour
{
    public static AnimatedImagePool ins;
    private void Awake()
    {
        ins = this;
    }
    [SerializeField] GameObject[] prefabs;
    public void Request(Vector2 rentPosition, Sprite sprite, float displayTime, int prefabIndex)
    {
        GameObject poolObj = SharedGameObjectPool.Rent(prefabs[prefabIndex]);
        poolObj.transform.SetParent(transform, false);
        poolObj.transform.localScale = Vector3.one;
        poolObj.transform.position = rentPosition;
        AnimatedImage instance;
        if(poolObj.TryGetComponent(out instance))
        {
            instance.Initialize(sprite, displayTime);
        }
    }
}