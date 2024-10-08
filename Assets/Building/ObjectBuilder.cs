using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    public UnityEvent<GameObject> onObjectBuild;
    public UnityEvent<GameObject> onObjectDeleted;
    [ReorderableList] public GameObject[] prefabs;
    int prefabIndex;
    bool canBuild;
    private void Update()
    {
        canBuild = MainCamera.ins.side == MainCamera.Side.Restaurant;
        if(!canBuild)
        {
            if (buildObjectInstance != null)
            {
                SharedGameObjectPool.Return(buildObjectInstance);
                buildObjectInstance = null;
            }
            toBuild = null;
            return;
        }

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
        debugGridPoint = gridPoint;
        debugBoxSize = Vector3.one /gridSize;

        toBuild.buildObjectInstance.transform.position = gridPoint;
        bool isOverlappingCorrectArea;
        bool isOverlappingIncorrectArea;
        toBuild.CheckOverlaps(out isOverlappingCorrectArea, out isOverlappingIncorrectArea);
        if (Input.GetMouseButton(0) && !OverCanvasCheck.ins.CheckOverCanvas() && isOverlappingCorrectArea && !isOverlappingIncorrectArea)
        {
            BuildObject();
        }

        if(Input.GetMouseButton(1))
        {
            //deletion
            Collider2D overlapResult = Physics2D.OverlapBox(debugGridPoint, debugBoxSize, 0f, RestaurantParameters.ins.AllBuiltObjectsLayerMask);
            if (overlapResult != null)
            {
                IBuiltObject toDelete = overlapResult.GetComponentInParent<IBuiltObject>();
                if (toDelete != null)
                {
                    toDelete.Remove();
                }
            }
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
        for(int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] == newPrefab)
            {
                prefabIndex = i;
                break;
            }
        }
    }
    protected void BuildObject()
    {
        if(RestaurantParameters.ins.TryBuyItem(toBuild.itemCost))
        {
            toBuild.buildObjectInstance.transform.parent = buildArea;
            toBuild.SetBuilder(this, prefabIndex);
            toBuild.Build();

            onObjectBuild?.Invoke(toBuild.buildObjectInstance);

            toBuild = null;

            NavMeshManager.ins.UpdateNavMesh();
        }
    }
    public void RemoveObject(IBuiltObject builtObject)
    {
        onObjectDeleted?.Invoke(builtObject.buildObjectInstance);

        SharedGameObjectPool.Return(builtObject.buildObjectInstance);

        NavMeshManager.ins.UpdateNavMesh();
    }
    public void BuildModeToggle(bool value)
    {
        if (!value)
        {
            if (toBuild != null)
            {
                if (buildObjectInstance != null)
                {
                    SharedGameObjectPool.Return(buildObjectInstance);
                    buildObjectInstance = null;
                }
                toBuild = null;
            }
        }
        inBuildMode = value;
    }
    public void ConstructSavedBuiltObject(int index, Vector3 worldPosition)
    {
        GameObject prefabToBuild = prefabs[index];
        GameObject poolObj = SharedGameObjectPool.Rent(prefabToBuild);
        IBuiltObject builtObjectInterface;
        if(poolObj.TryGetComponent(out builtObjectInterface))
        {
            poolObj.transform.parent = buildArea;
            poolObj.transform.position = worldPosition;
            builtObjectInterface.SetBuilder(this, index);
            builtObjectInterface.Build();

            onObjectBuild?.Invoke(poolObj);

        }
        NavMeshManager.ins.UpdateNavMesh();
    }
    Vector3 debugGridPoint;
    Vector3 debugBoxSize;
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(debugGridPoint, debugBoxSize);
    }
}