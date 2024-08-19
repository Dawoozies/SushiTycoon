using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;

public class KitchenObject : BuiltObject
{
    public enum ObjectID
    {
        ServingCounter,
        Fridge,
    }
    public ObjectID objectId;
    public override void Build()
    {
        base.Build();
        KitchenObjects.ins.AddObject(this, objectId);
    }
    public override void Remove()
    {
        base.Remove();
        KitchenObjects.ins.RemoveObject(this);
    }
}
