using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerVolumeEvents : MonoBehaviour
{
    [ReorderableList]
    public string[] validTags;
    public List<Action<Collider2D>> onEnterActions = new();
    public List<Action<Collider2D>> onStayActions = new();
    public List<Action<Collider2D>> onExitActions = new();
    [SerializeField] bool debugging;
    public void RegisterCollisionCallback(Action<Collider2D> a, CollisionEventType evtType)
    {
        switch (evtType)
        {
            case CollisionEventType.Enter:
                onEnterActions.Add(a);
                break;
            case CollisionEventType.Stay:
                onStayActions.Add(a);
                break;
            case CollisionEventType.Exit:
                onExitActions.Add(a);
                break;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(debugging)
        {
            Debug.Log($"{transform.name} detected {other.name} Enter");
        }
        if (!CheckValidTag(other))
            return;
        foreach (var a in onEnterActions)
        {
            a.Invoke(other);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if(debugging)
        {
            Debug.Log($"{transform.name} detected {other.name} Stay");
        }
        if (!CheckValidTag(other))
            return;
        foreach (var a in onStayActions)
        {
            a.Invoke(other);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (debugging)
        {
            Debug.Log($"{transform.name} detected {other.name} Exit");
        }
        if (!CheckValidTag(other))
            return;
        foreach (var a in onExitActions)
        {
            a.Invoke(other);
        }
    }
    protected virtual bool CheckValidTag(Collider2D other)
    {
        foreach (var tag in validTags)
        {
            if (other.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }
}
public enum CollisionEventType
{
    Enter, Stay, Exit
}