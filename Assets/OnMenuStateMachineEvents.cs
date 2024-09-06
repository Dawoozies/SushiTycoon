using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnMenuStateMachineEvents : MonoBehaviour
{
    MenuStateMachine menuStateMachine;
    public UnityEvent onMenuStateOpen;
    public UnityEvent onMenuStateClosed;
    void Start()
    {
        menuStateMachine = GetComponentInParent<MenuStateMachine>();
        menuStateMachine.AddListener(OnStateChanged);
    }
    void OnStateChanged(MenuStateMachine.MenuState state)
    {
        if(state == MenuStateMachine.MenuState.Open)
        {
            onMenuStateOpen?.Invoke();
        }
        else
        {
            onMenuStateClosed?.Invoke();
        }
    }
}
