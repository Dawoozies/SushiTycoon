using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpriteMoveFlip : MonoBehaviour
{
    public enum MoveType
    {
        None, NavMeshAgent, Rigidbody
    }
    [SerializeField] Transform graphic;
    NavMeshAgent agent;
    Rigidbody2D rb;
    MoveType moveType;
    private void Start()
    {
        moveType = MoveType.None;
        agent = GetComponentInParent<NavMeshAgent>();
        if (agent != null)
            moveType = MoveType.NavMeshAgent;
        rb = GetComponentInParent<Rigidbody2D>();
        if (rb != null)
            moveType = MoveType.Rigidbody;
    }
    private void Update()
    {
        Vector3 localScale = graphic.transform.localScale;
        switch (moveType)
        {
            case MoveType.NavMeshAgent:
                AgentUpdate(ref localScale);
                break;
            case MoveType.Rigidbody:
                RigidbodyUpdate(ref localScale);
                break;
        }
        graphic.transform.localScale = localScale;
    }
    void AgentUpdate(ref Vector3 localScale)
    {
        if (agent.velocity.x > 0)
        {
            localScale.x = -1;
        }
        if (agent.velocity.x < 0)
        {
            localScale.x = 1;
        }
    }
    void RigidbodyUpdate(ref Vector3 localScale)
    {
        if (rb.velocity.x > 0)
        {
            localScale.x = -1;
        }
        if (rb.velocity.x < 0)
        {
            localScale.x = 1;
        }
    }
}
