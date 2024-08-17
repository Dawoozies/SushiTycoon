using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tables : MonoBehaviour
{
    public static Tables ins;
    private void Awake()
    {
        ins = this;
    }
    List<Table> freeTables = new();
    public void AddTable(Table table)
    {
        freeTables.Add(table);
    }
    public void RemoveTable(Table table)
    {
        if(freeTables.Contains(table))
            freeTables.Remove(table);
    }
    public bool TryGetRandomTable(out Table table)
    {
        table = null;
        if (freeTables.Count == 0)
            return false;
        table = freeTables[Random.Range(0,freeTables.Count)];
        return true;
    }
}
