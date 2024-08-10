using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResourceBar : CanvasObject
{
    Slider bar;
    [SerializeField, ReorderableList] 
    Image[] images;
    Func<ResourceBarArgs> resourceBarArgsFetch;
    bool argsFetchAssigned;
    public override void OnReturn()
    {
        resourceBarArgsFetch = null;
        argsFetchAssigned = false;
    }
    public virtual void SetArgsFetch(Func<ResourceBarArgs> fetchFunc)
    {
        resourceBarArgsFetch = fetchFunc;
        argsFetchAssigned = true;
    }
    protected override void Start()
    {
        base.Start();
        bar = GetComponent<Slider>();
    }
    protected override void Update()
    {
        if (!argsFetchAssigned)
            return;
        ResourceBarArgs args = resourceBarArgsFetch.Invoke();
        position = args.worldPos;
        bar.maxValue = args.maxValue;
        bar.value = args.value;

        float valuePercentage = args.value / args.maxValue;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = args.gradients[i].Evaluate(valuePercentage);
        }
    }
}
public class ResourceBarArgs
{
    public Vector3 worldPos;
    public float value;
    public float maxValue;
    public Gradient[] gradients;
}