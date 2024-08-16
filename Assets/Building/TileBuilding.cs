using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuilding : MonoBehaviour
{
    public static TileBuilding ins;
    private void Awake()
    {
        ins = this;
    }
}
