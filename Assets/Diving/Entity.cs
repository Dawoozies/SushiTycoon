using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    [SerializeField] Transform graphicParent;
    NavMeshAgent agent;
    public UnityEvent onEntitySpawned;
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    protected virtual void Update()
    {
        Vector3 localScale = graphicParent.transform.localScale;
        if (agent.velocity.x > 0)
        {
            localScale.x = -1;
        }
        if(agent.velocity.x < 0)
        {
            localScale.x = 1;
        }
        graphicParent.transform.localScale = localScale;
    }
}
