using NavMeshPlus.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public static Building ins;
    void Awake()
    {
        ins = this;
    }
    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] GameObject prefab;
    [SerializeField] Color selectedForBuildColorTint;
    [SerializeField] Color selectedForBuildOverlapColorTint;
    [SerializeField] GameObject selectedForBuild;
    OnBuildingEvents onBuildingEvents;
    Action<Vector2> whileNotBuilt;
    Action onBuild;
    void Start()
    {
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        SelectForBuild(prefab);
    }
    void Update()
    {
        Vector2 mouseWorldPos = MainCamera.ins.mouseWorldPos;

        if (selectedForBuild == null)
            return;

        bool canBuild = true;
        if (onBuildingEvents != null)
        {
            canBuild = !onBuildingEvents.BuildingOverlapCheck();
            onBuildingEvents.ChangeColorTint(canBuild ? selectedForBuildColorTint : selectedForBuildOverlapColorTint);
        }

        if(Input.GetMouseButtonDown(0) && canBuild)
        {
            onBuild?.Invoke();
            selectedForBuild.transform.parent = navMeshSurface.transform;
            selectedForBuild = null;
            onBuildingEvents = null;
            //we dont destroy selected for build cause we build it
            onBuild = null;
            whileNotBuilt = null;

            SelectForBuild(prefab);
            return;
        }

        whileNotBuilt?.Invoke(mouseWorldPos);
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
    public void SelectForBuild(GameObject prefabToBuild)
    {
        if (selectedForBuild != null)
            Destroy(selectedForBuild);
        selectedForBuild = Instantiate(prefabToBuild);
        onBuildingEvents = selectedForBuild.GetComponentInChildren<OnBuildingEvents>();
        onBuildingEvents.SelectedForBuilding();
        onBuild = onBuildingEvents.Build;
        whileNotBuilt = onBuildingEvents.WhileNotBuilt;
    }
}
