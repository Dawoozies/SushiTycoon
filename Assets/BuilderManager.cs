using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;
public class BuilderManager : MonoBehaviour
{
    public static BuilderManager ins;
    void Awake()
    {
        ins = this;
    }
    public enum BuildCategory
    {
        Flooring,
        Objects,
        Zoning,
    }
    [SerializeField] BuildCategory category;
    [SerializeField] ObjectBuilder[] objectBuilders;
    int currentBuilder;
    Action<GameObject[]> onChangeBuildCategory;
    public void ChangeBuildCategory(BuildCategory value)
    {
        category = value;
        int builderIndex = (int)value;
        builderIndex %= objectBuilders.Length;
        for (int i = 0; i < objectBuilders.Length; i++)
        {
            objectBuilders[i].BuildModeToggle(i == builderIndex);
        }

        onChangeBuildCategory?.Invoke(objectBuilders[builderIndex].prefabs);
        currentBuilder = builderIndex;
    }
    public void ChangePrefabToBuild(GameObject prefab)
    {
        objectBuilders[currentBuilder].ChangePrefabToBuild(prefab);
    }
    public void RegisterOnChangeBuildCategoryCallback(Action<GameObject[]> a)
    {
        onChangeBuildCategory = a;
    }
}
