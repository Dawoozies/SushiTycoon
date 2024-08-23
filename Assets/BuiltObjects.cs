using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BayatGames.SaveGameFree;
public class BuiltObjects : MonoBehaviour
{
    public static BuiltObjects ins;
    private void Awake()
    {
        ins = this;
        //load the data
        LoadBuiltObjects();
    }
    public enum ObjectBuilderOrigin
    {
        FlooringBuilder,
        ObjectsBuilder,
        ZoningBuilder,
    }
    [SerializeField, ReorderableList] ObjectBuilder[] objectBuilders;
    [SerializeField, ReorderableList] List<BuiltObject> allBuiltObjects = new();
    public void AddToBuiltObjects(ObjectBuilder objectBuilder, BuiltObject builtObject)
    {
        allBuiltObjects.Add(builtObject);
        SaveBuiltObjects();
    }
    public void RemoveFromBuiltObjects(BuiltObject builtObject)
    {
        allBuiltObjects.Remove(builtObject);
        SaveBuiltObjects();
    }
    public void OnApplicationQuit()
    {
        if(!Application.isEditor)
        {
            SaveBuiltObjects();
        }
    }
    public void SaveBuiltObjects()
    {
        List<BuiltObjectSaveData> saveData = new();
        foreach (BuiltObject builtObject in allBuiltObjects)
        {
            BuiltObjectSaveData data = new();
            for (int i = 0; i < objectBuilders.Length; i++)
            {
                if (objectBuilders[i] == builtObject.builder)
                {
                    data.ObjectBuilderIndex = i;
                    break;
                }
            }
            data.ObjectBuilderPrefabIndex = builtObject.builderPrefabIndex;
            data.WorldPosition = builtObject.transform.position;
            saveData.Add(data);
        }
        SaveGame.Save<List<BuiltObjectSaveData>>("BuiltObjectsSaveData", saveData);
    }
    void LoadBuiltObjects()
    {
        if (!SaveGame.Exists("BuiltObjectsSaveData"))
            return;
        List<BuiltObjectSaveData> saveData = SaveGame.Load<List<BuiltObjectSaveData>>("BuiltObjectsSaveData");
        if (saveData != null && saveData.Count > 0)
        {
            foreach (BuiltObjectSaveData data in saveData)
            {
                Debug.LogError($"objBuilderIndex = {data.ObjectBuilderIndex} objBuilderPrefabIndex = {data.ObjectBuilderPrefabIndex} worldPos = {data.WorldPosition}");
                objectBuilders[data.ObjectBuilderIndex].ConstructSavedBuiltObject(data.ObjectBuilderPrefabIndex, data.WorldPosition);
            }
        }
    }
}
[Serializable]
public class BuiltObjectSaveData
{
    public int ObjectBuilderIndex;
    public int ObjectBuilderPrefabIndex;
    public Vector3 WorldPosition;
}