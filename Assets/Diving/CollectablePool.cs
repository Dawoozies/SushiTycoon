using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;
public abstract class CollectablePool : ObjectPoolBase<Collectable>
{
    protected override Collectable CreateInstance()
    {
        Collectable instance = new Collectable();
        return instance;
    }
    protected override void OnDestroy(Collectable instance)
    {
    }
    protected override void OnRent(Collectable instance)
    {
    }
    protected override void OnReturn(Collectable instance)
    {
    }
}