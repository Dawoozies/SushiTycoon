using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MoveInDirectionNavigator : Navigator
{
    [SerializeField] Vector3 moveDirection;
    public override void Navigate()
    {
        if (!movementAllowed || !isActiveNavigator)
            return;
        Vector3 p = transform.position + moveDirection;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(p, out hit, 5f, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(hit.position, path);
            agent.path = path;
        }
    }
    protected override void Update()
    {
        base.Update();
    }
}
