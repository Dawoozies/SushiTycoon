using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderSwapper : MonoBehaviour
{
    ObjectBuilder[] objectBuilders;
    int currentIndex;
    private void Start()
    {
        objectBuilders = GetComponentsInChildren<ObjectBuilder>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            objectBuilders[currentIndex].BuildModeToggle(false);
            currentIndex++;
            currentIndex %= objectBuilders.Length;
            objectBuilders[currentIndex].BuildModeToggle(true);
        }
    }
}
