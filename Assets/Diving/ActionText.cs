using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class ActionText : CanvasObject
{
    TextMeshProUGUI text;
    Func<ActionTextArgs> actionTextArgsFetch;
    bool argsFetchAssigned;
    public override void OnReturn()
    {
        actionTextArgsFetch = null;
        argsFetchAssigned = false;
    }
    public virtual void SetArgsFetch(Func<ActionTextArgs> fetchFunc)
    {
        actionTextArgsFetch = fetchFunc;
        argsFetchAssigned = true;
    }
    protected override void Start()
    {
        base.Start();
        text = GetComponent<TextMeshProUGUI>();
    }
    protected override void Update()
    {
        if (!argsFetchAssigned)
        {
            text.text = "";
            return;
        }
        ActionTextArgs args = actionTextArgsFetch.Invoke();
        position = args.worldPos;
        for (int i = 0; i < args.textLines.Length; i++)
        {
            if(i == 0)
            {
                text.text = args.textLines[i];
            }
            else
            {
                text.text += $"\n{args.textLines[i]}";
            }
        }
    }
}
public class ActionTextArgs
{
    public Vector3 worldPos;
    public string[] textLines;
}
