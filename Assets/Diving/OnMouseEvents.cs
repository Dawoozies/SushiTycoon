using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseEvents : MonoBehaviour
{
    MouseEventArgs mouseEventArgs = new();
    public List<Action<MouseEventArgs>> onEnterActions = new();
    public List<Action<MouseEventArgs>> onOverActions = new();
    public List<Action<MouseEventArgs>> onExitActions = new();
    public void RegisterOnMouseEventCallback(Action<MouseEventArgs> a, MouseEventType evtType)
    {
        switch (evtType)
        {
            case MouseEventType.Enter:
                onEnterActions.Add(a);
                break;
            case MouseEventType.Over:
                onOverActions.Add(a);
                break;
            case MouseEventType.Exit:
                onExitActions.Add(a);
                break;
        }
    }
    private void Update()
    {
        mouseEventArgs.MouseScreenPosition = Input.mousePosition;
    }
    private void OnMouseDown()
    {
        
    }
    private void OnMouseDrag()
    {
        
    }
    private void OnMouseEnter()
    {
        
    }
    private void OnMouseExit()
    {
        
    }
    private void OnMouseOver()
    {
        
    }
    private void OnMouseUp()
    {
        
    }
    private void OnMouseUpAsButton()
    {
        
    }
}
public enum MouseEventType
{
    Down, Drag, Enter, Exit, Over, Up, UpAsButton
}
public class MouseEventArgs
{
    public Vector2 MouseScreenPosition;
    public Vector2 MouseScreenDelta;
    public Vector3 MouseWorldPosition;
    public Vector3 MouseWorldDelta;
    public InputState LeftMouseButton;
    public InputState RightMouseButton;
    public InputState MiddleMouseButton;
    public enum InputState
    {
        None, Down, Held, Up
    }
}