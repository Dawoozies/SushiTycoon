using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSwapper : MonoBehaviour
{
    public ObjectBuilder builder;
    public GameObject[] prefabs;
    int currentPrefab;
    int _currentPrefab;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            currentPrefab--;
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            currentPrefab++;
        }
        currentPrefab = currentPrefab % prefabs.Length;
        if (currentPrefab < 0)
            currentPrefab = prefabs.Length - 1;

        if(_currentPrefab != currentPrefab)
        {
            _currentPrefab = currentPrefab;
            builder.ChangePrefabToBuild(prefabs[_currentPrefab]);
        }
    }
}
