using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class CollectableHoverOverPool : MonoBehaviour
{
    public static CollectableHoverOverPool ins;
    void Awake()
    {
        ins = this;
    }
    public GameObject prefab;
    void Start()
    {
    }
    public void Request(CollectableData collectable)
    {
        //many things can have something like an inspection window.
        //maybe make the UI less specific and have ways of having different types of UI pop ups and such
    }
}
