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
    [SerializeField] SerializedDictionary<Table.State, List<Table>> tables = new();
    public void OnObjectBuild(GameObject builtObject)
    {
        Table table = builtObject.GetComponent<Table>();
    }
    public void OnObjectDeleted(GameObject deletedObject)
    {
        Table table = deletedObject.GetComponent<Table>();
    }
    public void AddTable(Table table, Table.State state)
    {
        if(tables.ContainsKey(state))
        {
            tables[state].Add(table);
        }
        else
        {
            tables.Add(state, new List<Table> { table });
        }
    }
    public void RemoveTable(Table table)
    {
        foreach (Table.State key in tables.Keys)
        {
            if (tables[key].Contains(table))
            {
                tables[key].Remove(table);
                break;
            }
        }
    }
    public void SwapState(Table table, Table.State oldState, Table.State newState)
    {
        //get the old state list
        // iff old state list contains table
        // => remove from list
        if(tables.ContainsKey(oldState))
        {
            if (tables[oldState].Contains(table))
                tables[oldState].Remove(table);
        }
        if (!tables.ContainsKey(newState))
        {
            AddTable(table, newState);
            return;
        }
        //then get the new state list
        // => add to list
        tables[newState].Add(table);
    }
    public bool TryGetRandomTable(Table.State state, out Table table)
    {
        table = null;
        if (!tables.ContainsKey(state))
            return false;
        if (tables[state] == null || tables[state].Count == 0)
            return false;

        table = tables[state][Random.Range(0, tables[state].Count)];
        return true;
    }
}
