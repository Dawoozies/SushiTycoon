using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class CollectableData : ScriptableObject
{
    public GameObject entityBase;
    public Sprite sprite;
    public Vector2 boxColliderOffset;
    public Vector2 boxColliderSize;
    public float collectionTime;
    public float weight;
    public int rank;
}