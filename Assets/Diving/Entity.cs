using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    NavMeshAgent agent;
    public UnityEvent onEntitySpawned;
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    protected virtual void Update()
    {
    }
}
