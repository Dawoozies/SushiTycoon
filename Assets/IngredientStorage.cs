using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientStorage : MonoBehaviour
{
    public static IngredientStorage ins;
    private void Awake()
    {
        ins = this;
    }
    [SerializeField] SerializedDictionary<CollectableData, int> ingredients = new();
    public void AddToStorage(CollectableData collectable, int amount)
    {
        ingredients.TryAdd(collectable, amount);
    }
}
