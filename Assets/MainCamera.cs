using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera ins;
    Camera c;
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
    public enum Side
    {
        Restaurant,
        Diving
    }
    public Side side;
    public Side currentSideBeingDragged;
    private void Awake()
    {
        ins = this;
        c = GetComponentInChildren<Camera>();
    }
    public Vector3 WorldToScreenSpace(Vector3 worldPos)
    {
        Vector3 screenPos = sideCameras[(int)side].WorldToScreenPoint(worldPos);
        screenPos.z = 0f;
        return screenPos;
    }
    public Vector3 ScreenToWorldSpace(Vector3 screenPos)
    {
        Vector3 worldPos = sideCameras[(int)side].ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        return worldPos;
    }
    private void Update()
    {
        //click and drag?
        float screenWidth = c.scaledPixelWidth;

        mouseScreenPos = Input.mousePosition;

        if (mouseScreenPos.x > screenWidth / 2f)
        {
            side = Side.Diving;
        }
        else
        {
            side = Side.Restaurant;
        }

        mouseWorldPos = sideCameras[(int)side].ScreenToWorldPoint(mouseScreenPos);
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
    }
    void CameraMovement()
    {
        if(!middleMouse)
        {
            return;
        }
        sideCameras[(int)side].transform.Translate(-mouseDelta * (cameraMoveSensitivity*cameraMoveSensitivityCurve.Evaluate(Mathf.InverseLerp(orthographicSizeBounds.x, orthographicSizeBounds.y, c.orthographicSize))));
    }
    void CameraZoom()
    {
        sideCameras[(int)side].orthographicSize -= mouseScrollDelta.y;
        sideCameras[(int)side].orthographicSize = Mathf.Clamp(c.orthographicSize,orthographicSizeBounds.x,orthographicSizeBounds.y);

/*        cinemachineVirtualCamera.m_Lens.OrthographicSize -= mouseScrollDelta.y;
        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(cinemachineVirtualCamera.m_Lens.OrthographicSize, orthographicSizeBounds.x, orthographicSizeBounds.y);*/
    }
}