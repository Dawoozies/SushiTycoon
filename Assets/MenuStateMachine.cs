using LitMotion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityHFSM;

public class MenuStateMachine : MonoBehaviour
{
    [SerializeField] MenuState state;
    StateMachine<MenuState> menuStateMachine = new StateMachine<MenuState>();
    [SerializeField] RectTransform menuRect;
    [SerializeField] float transitionTime;
    [SerializeField] Ease openEasing;
    [SerializeField] Ease closeEasing;
    [SerializeField] RectTransform closedRect;
    [SerializeField] RectTransform openRect;
    MotionHandle motionHandle;
    public UnityEvent<MenuState> onStateChanged;
    public enum MenuState
    {
        Closed,
        Open,
    }
    void Start()
    {
        menuStateMachine.AddState(MenuState.Closed, new State<MenuState>(onLogic: state => { }));
        menuStateMachine.AddState(MenuState.Open, new State<MenuState>(onLogic: state => { }));

        menuStateMachine.AddTransition(MenuState.Closed, MenuState.Open, t => state == MenuState.Open);
        menuStateMachine.AddTransition(MenuState.Open, MenuState.Closed, t => state == MenuState.Closed);

        menuStateMachine.StateChanged += OnStateChanged;

        menuStateMachine.Init();

        menuStateMachine.RequestStateChange(state);
    }
    void OnStateChanged(StateBase<MenuState> state)
    {
        Debug.LogError($"change state to {state.name}");
        if(state.name == MenuState.Open)
        {
            motionHandle = LMotion.Create(closedRect.localPosition, openRect.localPosition, transitionTime)
                .WithEase(openEasing)
                .Bind(x => menuRect.localPosition = x);
            LMotion.Create(closedRect.rect.width, openRect.rect.width, transitionTime)
                .WithEase(openEasing)
                .Bind(x => menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x));
            LMotion.Create(closedRect.rect.height, openRect.rect.height, transitionTime)
                .WithEase(openEasing)
                .Bind(x => menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x));
        }
        else
        {
            motionHandle = LMotion.Create(openRect.localPosition, closedRect.localPosition, transitionTime)
                .WithEase(closeEasing)
                .Bind(x => menuRect.localPosition = x);
            LMotion.Create(openRect.rect.width, closedRect.rect.width, transitionTime)
                .WithEase(closeEasing)
                .Bind(x => menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x));
            LMotion.Create(openRect.rect.height, closedRect.rect.height, transitionTime)
                .WithEase(closeEasing)
                .Bind(x => menuRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x));
        }
    }
    public void ChangeState()
    {
        if(motionHandle != null && motionHandle.IsActive())
            return;
        switch (state)
        {
            case MenuState.Closed:
                state = MenuState.Open;
                break;
            case MenuState.Open:
                state = MenuState.Closed;
                break;
        }

        menuStateMachine.RequestStateChange(state);
        onStateChanged?.Invoke(state);
    }
}
