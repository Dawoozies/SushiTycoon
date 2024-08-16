using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;
public class ObjectBuilder : MonoBehaviour
{
    //put a prefab in
    [SerializeField] Transform buildArea;
    [SerializeField] GameObject prefabToBuild;
    IBuiltObject toBuild;
    [SerializeField] float rotationAngle;
    [SerializeField] bool inBuildMode;
    [SerializeField] float gridSize;
    [SerializeField] bool snapToGrid;
    bool builtThisFrame;

    GameObject buildObjectInstance;
    private void Update()
    {
        if (!inBuildMode)
            return;

        if (prefabToBuild == null)
            return;

        if(toBuild == null)
        {
            GetNewInstanceToBuild();
        }

        Vector2 mouseWorldPos = MainCamera.ins.mouseWorldPos;
        Vector2 mouseScrollDelta = Input.mouseScrollDelta;
        Vector3 gridPoint = mouseWorldPos;
        if(snapToGrid)
        {
            gridPoint.x = Mathf.RoundToInt(gridPoint.x*gridSize)/gridSize;
            gridPoint.y = Mathf.RoundToInt(gridPoint.y*gridSize)/gridSize;
        }
        toBuild.buildObjectInstance.transform.position = gridPoint;
        bool isOverlappingCorrectArea;
        bool isOverlappingIncorrectArea;
        toBuild.CheckOverlaps(out isOverlappingCorrectArea, out isOverlappingIncorrectArea);
        if (Input.GetMouseButton(0) && isOverlappingCorrectArea && !isOverlappingIncorrectArea)
        {
            BuildObject();
        }
    }
    void GetNewInstanceToBuild()
    {
        buildObjectInstance = SharedGameObjectPool.Rent(prefabToBuild);
        toBuild = buildObjectInstance.GetComponent<IBuiltObject>();
    }
    public void ChangePrefabToBuild(GameObject newPrefab)
    {
        if(buildObjectInstance != null)
        {
            SharedGameObjectPool.Return(buildObjectInstance);
        }

        if(prefabToBuild != null)
        {
            prefabToBuild = null;
            toBuild = null;
        }

        prefabToBuild = newPrefab;
    }
    protected void BuildObject()
    {
        toBuild.buildObjectInstance.transform.parent = buildArea;
        toBuild.Build();
        toBuild = null;

        NavMeshManager.ins.UpdateNavMesh();
    }
}