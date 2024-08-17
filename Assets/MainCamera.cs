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
        mouseScreenPos = Input.mousePosition;
        mouseWorldPos = c.ScreenToWorldPoint(mouseScreenPos);
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
        }
        if(Input.GetMouseButtonUp(2))
        {
            middleMouse = false;
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
        transform.Translate(-mouseDelta * (cameraMoveSensitivity*cameraMoveSensitivityCurve.Evaluate(Mathf.InverseLerp(orthographicSizeBounds.x, orthographicSizeBounds.y, c.orthographicSize))));
    }
    void CameraZoom()
    {
        c.orthographicSize -= mouseScrollDelta.y;
        c.orthographicSize = Mathf.Clamp(c.orthographicSize,orthographicSizeBounds.x,orthographicSizeBounds.y);
    }
}