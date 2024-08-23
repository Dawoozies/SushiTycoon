using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasWindowManager : MonoBehaviour
{
    //canvas windows
    ICanvasMenu[] menus;
    void Start()
    {
        menus = GetComponentsInChildren<ICanvasMenu>();
    }
}
