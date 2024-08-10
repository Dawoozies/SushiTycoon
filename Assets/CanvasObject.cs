using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPools;
public class CanvasObject : MonoBehaviour, IPoolCallbackReceiver
{
    RectTransform rectTransform;
    public Vector3 position { 
        get {
            return MainCamera.ins.ScreenToWorldSpace(rectTransform.localPosition);
        }
        set { 
            rectTransform.position = MainCamera.ins.WorldToScreenSpace(value);
        } 
    }
    public Vector2 size {
        set {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
        }
    }
    public virtual void OnRent()
    {
    }
    public virtual void OnReturn()
    {
    }
    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    protected virtual void Update()
    {
    }
}
