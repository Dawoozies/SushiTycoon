using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class CollectableData : ScriptableObject
{
    public GameObject entityBase;
    public float collectionTime;
    public float weight;
}