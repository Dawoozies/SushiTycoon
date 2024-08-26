using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera ins;
    public Camera cam;
    public Vector2 mouseScreenPos;
    public Vector2 mouseWorldPos;
    public Vector2 mouseDelta;
    public Vector2 mouseScrollDelta;
    bool middleMouse;
    public Vector2 orthographicSizeBounds;
    public float cameraMoveSensitivity;
    public AnimationCurve cameraMoveSensitivityCurve;

    [SerializeField] float maximumMouseDelta;
    [SerializeField] Camera[] sideCameras;
    [SerializeField] Transform[] sideCameraBoundParents;
    public enum Side
    {
        Restaurant,
        Diving
    }
    public Side side;
    public Side currentSideBeingDragged;
    Vector2 cameraShift;
    private void Awake()
    {
        ins = this;
    }
    public Vector3 WorldToScreenSpace(Vector3 worldPos)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
        screenPos.z = 0f;
        return screenPos;
    }
    public Vector3 WorldToSideScreenSpace(Vector3 worldPos, Side sideCamera)
    {
        Vector3 cameraSideDifference = sideCameras[(int)sideCamera].transform.position -cam.transform.position;
        Vector3 p = worldPos - cameraSideDifference;
        Vector3 screenPos = cam.WorldToScreenPoint(p);
        Vector3 sideShift = Vector3.zero;
        float screenWidth = cam.scaledPixelWidth;
        switch (sideCamera)
        {
            case Side.Restaurant:
                sideShift = -Vector3.right * screenWidth / 2f;
                break;
            case Side.Diving:
                sideShift = Vector3.right * screenWidth / 2f;
                break;
        }
        screenPos.z = 0f;
        return screenPos + sideShift;
    }
    public Vector3 ScreenToWorldSpace(Vector3 screenPos)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint((Vector2)screenPos + cameraShift);
        worldPos.z = 0f;
        return worldPos;
    }
    public Camera GetSideCamera(Side side)
    {
        return sideCameras[(int)side];
    }
    public float SideCameraScalingFactor(Side side)
    {
        Camera sideCam = sideCameras[(int)side];
        float halfHeight = sideCam.orthographicSize;
        float halfWidth = halfHeight * sideCam.aspect;
        float minHalfHeight = orthographicSizeBounds.x;
        float maxHalfHeight = orthographicSizeBounds.y;
        return Mathf.InverseLerp(minHalfHeight,maxHalfHeight,halfHeight);
    }
    private void Update()
    {
        //click and drag?
        float screenWidth = cam.scaledPixelWidth;

        mouseScreenPos = Input.mousePosition;
        cameraShift = Vector2.zero;
        if (mouseScreenPos.x > screenWidth / 2f)
        {
            side = Side.Diving;
            cameraShift = -Vector2.right * screenWidth / 2f;

        }
        else
        {
            side = Side.Restaurant;
            cameraShift = Vector2.right * screenWidth / 2f;
        }
        cam.transform.position = sideCameras[(int)side].transform.position;
        cam.orthographicSize = sideCameras[(int)side].orthographicSize;

        mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos + cameraShift);
        mouseDelta.x = Input.GetAxis("Mouse X");
        mouseDelta.y = Input.GetAxis("Mouse Y");

        if(mouseDelta.magnitude > maximumMouseDelta)
        {
            Debug.Log($"mouseDelta = {mouseDelta} exceeds maximum magnitude of {maximumMouseDelta}");
            return;
        }

        mouseScrollDelta = Input.mouseScrollDelta;

        if (Input.GetMouseButtonDown(2))
        {
            middleMouse = true;
            currentSideBeingDragged = side;
        }
        if(Input.GetMouseButtonUp(2))
        {
            middleMouse = false;
            currentSideBeingDragged = side;
        }

        CameraMovement();
        CameraZoom();
        CameraBounding();
    }
    void CameraBounding()
    {
        Transform camTransform = sideCameras[(int)currentSideBeingDragged].transform;
        Camera sideCam = sideCameras[(int)currentSideBeingDragged];
        float halfHeight = sideCam.orthographicSize;
        float halfWidth = halfHeight * sideCam.aspect;
        Transform bounds = sideCameraBoundParents[(int)currentSideBeingDragged];
        float camXPos = camTransform.position.x;
        // 0=right 1=left 2=up 3=down
        if (currentSideBeingDragged == Side.Restaurant)
        {
            camXPos = Mathf.Clamp(
                camTransform.position.x,
                bounds.GetChild(1).position.x,
                bounds.GetChild(0).position.x - halfWidth
                );
        }
        if(currentSideBeingDragged == Side.Diving)
        {
            camXPos = Mathf.Clamp(
                camTransform.position.x,
                bounds.GetChild(1).position.x + halfWidth,
                bounds.GetChild(0).position.x
                );
        }

        float camYPos = Mathf.Clamp(
            camTransform.position.y,
            bounds.GetChild(3).position.y + halfHeight,
            bounds.GetChild(2).position.y - halfHeight
            );
        camTransform.position = new Vector3(camXPos, camYPos, -10);
    }
    void CameraMovement()
    {
        if(!middleMouse)
        {
            return;
        }
        sideCameras[(int)currentSideBeingDragged].transform.Translate(-mouseDelta * (cameraMoveSensitivity*cameraMoveSensitivityCurve.Evaluate(Mathf.InverseLerp(orthographicSizeBounds.x, orthographicSizeBounds.y, sideCameras[(int)currentSideBeingDragged].orthographicSize))));

    }
    void CameraZoom()
    {
        sideCameras[(int)currentSideBeingDragged].orthographicSize -= mouseScrollDelta.y;
        sideCameras[(int)currentSideBeingDragged].orthographicSize = Mathf.Clamp(sideCameras[(int)currentSideBeingDragged].orthographicSize,orthographicSizeBounds.x,orthographicSizeBounds.y);

/*        cinemachineVirtualCamera.m_Lens.OrthographicSize -= mouseScrollDelta.y;
        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(cinemachineVirtualCamera.m_Lens.OrthographicSize, orthographicSizeBounds.x, orthographicSizeBounds.y);*/
    }
}