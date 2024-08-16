using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    public static NavMeshManager ins;
    NavMeshSurface navMeshSurface;
    bool builtThisFrame;
    private void Awake()
    {
        ins = this;
        navMeshSurface = GetComponent<NavMeshSurface>();
        UpdateNavMesh();
    }
    private void Start()
    {
        UpdateNavMesh();
    }
    public void UpdateNavMesh()
    {
        builtThisFrame = true;
    }
    private void LateUpdate()
    {
        if(builtThisFrame)
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            builtThisFrame = false;
        }
    }
}
