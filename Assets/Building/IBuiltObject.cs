using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IBuiltObject
{
    public GameObject buildObjectInstance { get; }
    //Who built this object
    ObjectBuilder builder { get; }
    SpriteLayer spriteLayer { get; }
    public void SetBuilder(ObjectBuilder builder);
    public void Build();
    public void CheckOverlaps(out bool isOverlappingCorrectArea, out bool isOverlappingIncorrectArea);
}