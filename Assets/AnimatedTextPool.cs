using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class AnimatedTextPool : MonoBehaviour
{
    public static AnimatedTextPool ins;
    private void Awake()
    {
        ins = this;
    }
    [SerializeField] GameObject prefab;
    private void Start()
    {
        SharedGameObjectPool.Prewarm(prefab, 20);
    }
    public void Request(Vector2 rentPosition, string textInput)
    {
        GameObject poolObj = SharedGameObjectPool.Rent(prefab);
        poolObj.transform.SetParent(transform, false);
        poolObj.transform.localScale = Vector3.one;
        poolObj.transform.position = rentPosition;
        AnimatedText instance;
        if (poolObj.TryGetComponent(out instance))
        {
            instance.Initialize(textInput);
        }
    }
}
