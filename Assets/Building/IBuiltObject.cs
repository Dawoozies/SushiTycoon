using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IBuiltObject
{
    public GameObject buildObjectInstance { get; }
    //Who built this object
    ObjectBuilder builder { get; }
    SpriteLayer spriteLayer { get; }
    public void SetBuilder(ObjectBuilder builder, int builderPrefabIndex);
    public void Build();
    public void CheckOverlaps(out bool isOverlappingCorrectArea, out bool isOverlappingIncorrectArea);
    public void Remove();
    public string itemName { get; }
    public Sprite sprite { get; }
    public float itemCost { get; }
}