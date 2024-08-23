using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigatorRandomWalk : Navigator
{
    [SerializeField] float stepRadius;
    [SerializeField] Vector2 speedBounds;
    [SerializeField] Vector2 waitTimeBounds;
    float _waitTime;
    public override void Navigate()
    {
        if (!movementAllowed || !isActiveNavigator || !isOnNavMesh)
            return;
        if (NeedNewPath() && _waitTime <= 0f)
        {
            Vector3 p = transform.position + Random.onUnitSphere * stepRadius;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(p, out hit, stepRadius, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(hit.position, path);
                agent.path = path;
            }

            _waitTime = Random.Range(waitTimeBounds.x, waitTimeBounds.y);
            agent.speed = Random.Range(speedBounds.x, speedBounds.y);
        }
    }
    protected override void Update()
    {
        base.Update();
        if(NeedNewPath() && _waitTime > 0f)
        {
            _waitTime -= Time.deltaTime;
        }
    }
}