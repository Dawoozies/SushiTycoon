using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuStateText : MonoBehaviour
{
    public TMP_Text text;
    public string[] texts;
    public void OnStateChangeHandler(MenuStateMachine.MenuState state)
    {
        text.text = texts[(int)state];
    }
}
