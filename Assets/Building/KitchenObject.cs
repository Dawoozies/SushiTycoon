using System;
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
        CookingStation,
        PrepTable,
    }
    public ObjectID objectId;
    public object user;
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
    public bool TryAssignUserToObject(object o, out Action onUserStopUseCallback)
    {
        onUserStopUseCallback = null;
        if (user != null && user != o)
            return false;

        user = o;
        onUserStopUseCallback = UserStopUseHandler;
        return true;
    }
    void UserStopUseHandler()
    {
        user = null;
    }
}
