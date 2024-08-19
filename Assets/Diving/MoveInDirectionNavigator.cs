using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MoveInDirectionNavigator : Navigator
{
    [SerializeField] Vector3 moveDirection;
    float originalSpeed;
    protected override void Start()
    {
        base.Start();
        originalSpeed = agent.speed;
    }
    public override void Navigate()
    {
        if (!agent.isOnNavMesh || !movementAllowed || !isActiveNavigator)
            return;
        Vector3 p = transform.position + moveDirection;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(p, out hit, 5f, (int)allowedAreas))
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
    public virtual void SetMoveDirection(Vector3 moveDir)
    {
        moveDirection = moveDir;
    }
}
