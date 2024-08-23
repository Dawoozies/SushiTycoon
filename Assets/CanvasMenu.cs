using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using TMPro;
using UnityHFSM;
using System;
using UnityEngine.Events;
public abstract class CanvasMenu : MonoBehaviour, ICanvasMenu
{
    StateMachine<int> fsm = new StateMachine<int>();
    [SerializeField] RectTransform menuRect;
    [SerializeField, ReorderableList, InLineEditor] MenuStateRect[] stateRects;
    MotionHandle menuMotionHandle;
    public int currentState;
    public int previousState;
    public UnityEvent<int> onStateChanged;
    public virtual void Start()
    {
        for (int i = 0; i < stateRects.Length; i++)
        {
            fsm.AddState(i, new State<int>());
        }
        fsm.StateChanged += OnStateChanged;
    }
    public virtual void ChangeState(int stateIndex)
    {
        if (menuMotionHandle != null && menuMotionHandle.IsActive())
            return;

        previousState = currentState;
        currentState = stateIndex;
        fsm.RequestStateChange(currentState);
    }
    public virtual void OnStateChanged(StateBase<int> state)
    {
        MenuStateRect a = stateRects[previousState];
        MenuStateRect b = stateRects[state.name];

        menuMotionHandle = LMotion.Create(a.rectTransform.localPosition, b.rectTransform.localPosition, b.time)
            .WithEase(b.easing)
            .Bind(x => menuRect.localPosition = x);
        LMotion.Create(a.rectTransform.rect.width, b.rectTransform.rect.width, b.time)
            .WithEase(b.easing)
            .Bind(x => menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x));
        LMotion.Create(a.rectTransform.rect.height, b.rectTransform.rect.height, b.time)
                .WithEase(b.easing)
                .Bind(x => menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x));
    }
}
public interface ICanvasMenu
{
    public void ChangeState(int stateIndex);
}
[Serializable]
public class MenuStateRect
{
    public RectTransform rectTransform;
    public float time;
    public Ease easing;
}