using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStateGameObjectActive : MonoBehaviour
{
    public GameObject target;
    public bool[] setActiveStates;
    public void OnStateChangeHandler(MenuStateMachine.MenuState state)
    {
        target.SetActive(setActiveStates[(int)state]);
    }
}
