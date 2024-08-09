using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpriteMoveFlip : MonoBehaviour
{
    [SerializeField] Transform graphic;
    NavMeshAgent agent;
    private void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
    }
    private void Update()
    {
        if (agent == null)
        {
            return;
        }
        Vector3 localScale = graphic.transform.localScale;
        if (agent.velocity.x > 0)
        {
            localScale.x = -1;
        }
        if (agent.velocity.x < 0)
        {
            localScale.x = 1;
        }
        graphic.transform.localScale = localScale;
    }
}
