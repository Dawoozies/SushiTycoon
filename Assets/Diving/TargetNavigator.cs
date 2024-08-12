using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetNavigator : Navigator
{
    [SerializeField] Transform target;
    public bool hasTarget;
    float originalSpeed;
    protected override void Start()
    {
        base.Start();
        originalSpeed = agent.speed;
    }
    public void ClearTarget()
    {
        target = null;
        hasTarget = false;
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
        hasTarget = true;
    }
    public float DistanceFromTarget()
    {
        if(!hasTarget)
        {
            return -1f;
        }
        return Vector2.Distance(transform.position, target.position);
    }
    public override bool NeedNewPath()
    {
        return !hasPath;
    }
    public override void Navigate()
    {
        if (!movementAllowed || !isActiveNavigator || !hasTarget)
            return;
        agent.speed = originalSpeed;
        Vector3 p = target.position;
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
        if(hasTarget)
        {
            if (target == null)
                hasTarget = false;
        }
        base.Update();
    }
}