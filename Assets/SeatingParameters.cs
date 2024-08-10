using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SeatingParameters : MonoBehaviour
{
    public static SeatingParameters ins;

    private void Awake()
    {
        ins = this;
    }

    public float SeatingDistance;
}
