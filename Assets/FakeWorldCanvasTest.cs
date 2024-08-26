using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class FakeWorldCanvasTest : CanvasObject
{
    public MainCamera.Side sideCamera;
    public Transform worldObjectToTrack;
    public Vector2 scalingFactorBounds;
    public override Vector3 position {
        get
        {
            if (worldCanvas)
            {
                return rectTransform.localPosition;
            }
            return MainCamera.ins.ScreenToWorldSpace(rectTransform.localPosition);
        }
        set
        {
            if (worldCanvas)
            {
                rectTransform.position = value;
                return;
            }
            rectTransform.position = MainCamera.ins.WorldToSideScreenSpace(value, sideCamera);
        }
    }
    Vector3 originalScale;
    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
    }
    protected override void Update()
    {
        position = worldObjectToTrack.position;
        Camera sideCam = MainCamera.ins.GetSideCamera(sideCamera);
        float halfHeight = sideCam.orthographicSize;
        float halfWidth = halfHeight * sideCam.aspect;
        float sideCameraScalingFactor = MainCamera.ins.SideCameraScalingFactor(sideCamera);
        sideCameraScalingFactor = Mathf.Clamp(sideCameraScalingFactor, scalingFactorBounds.x, scalingFactorBounds.y);
        transform.localScale = new Vector3(originalScale.x/sideCameraScalingFactor, originalScale.y/sideCameraScalingFactor,1f);
    }
}
