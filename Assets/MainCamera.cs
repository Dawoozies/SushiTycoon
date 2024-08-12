using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera ins;
    Camera c;
    private void Awake()
    {
        ins = this;
        c = GetComponent<Camera>();
    }
    public Vector3 WorldToScreenSpace(Vector3 worldPos)
    {
        Vector3 screenPos = c.WorldToScreenPoint(worldPos);
        screenPos.z = 0f;
        return screenPos;
    }
    public Vector3 ScreenToWorldSpace(Vector3 screenPos)
    {
        Vector3 worldPos = c.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        return worldPos;
    }
    private void Update()
    {
        //click and drag?
    }
}