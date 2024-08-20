using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObjects : MonoBehaviour
{
    public static KitchenObjects ins;
    private void Awake()
    {
        ins = this;
    }
    [SerializeField] SerializedDictionary<KitchenObject.ObjectID, List<KitchenObject>> kitchenObjects;
    public void AddObject(KitchenObject obj, KitchenObject.ObjectID id)
    {
        if(kitchenObjects.ContainsKey(id))
        {
            kitchenObjects[id].Add(obj);
        }
        else
        {
            kitchenObjects.Add(id, new List<KitchenObject> { obj });
        }
    }
    public void RemoveObject(KitchenObject obj)
    {
        foreach (KitchenObject.ObjectID key in kitchenObjects.Keys)
        {
            if (kitchenObjects[key].Contains(obj))
            {
                kitchenObjects[key].Remove(obj);
                break;
            }
        }
    }
    [SerializeField] SerializedDictionary<object, float> sqrDistLookUpTable = new Dictionary<object, float>();
    public bool TryGetClosestObjectWithID<T>(Vector2 point, KitchenObject.ObjectID id, out T component)
    {
        component = default(T);

        if (!kitchenObjects.ContainsKey(id))
            return false;

        if (kitchenObjects[id].Count == 0)
            return false;

        sqrDistLookUpTable.Clear();
        kitchenObjects[id].Sort((KitchenObject a, KitchenObject b) =>
        {
            float sqrDstA = 0f;
            float sqrDstB = 0f;
            if(!sqrDistLookUpTable.ContainsKey(a))
            {
                sqrDstA = ((Vector2)a.transform.position - point).sqrMagnitude;
                sqrDistLookUpTable.Add(a, sqrDstA);
            }
            else
            {
                sqrDstA = sqrDistLookUpTable[a];
            }
            if (!sqrDistLookUpTable.ContainsKey(b))
            {
                sqrDstB = ((Vector2)b.transform.position - point).sqrMagnitude;
                sqrDistLookUpTable.Add(b, sqrDstB);
            }
            else
            {
                sqrDstB = sqrDistLookUpTable[b];
            }
            return sqrDstA.CompareTo(sqrDstB);
        });

        //pretty sure after this the first
        if (!kitchenObjects[id][0].TryGetComponent<T>(out component))
        {
            return false;
        }

        return true;
    }
}