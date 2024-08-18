using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PointNavigator : Navigator
{
    [SerializeField] Vector3 point;
    public bool hasPoint;
    float originalSpeed;
    public bool nearPoint => Vector2.Distance(transform.position, point) < agent.stoppingDistance;
    protected override void Start()
    {
        base.Start();
        originalSpeed = agent.speed;
    }
    public void ClearPoint()
    {
        point = transform.position;
        hasPoint = false;
    }
    public void SetPoint(Vector3 point)
    {
        this.point = point;
        hasPoint = true;
    }
    public float DistanceFromPoint()
    {
        if (!hasPoint)
        {
            return -1f;
        }
        return Vector2.Distance(transform.position, point);
    }
    public override bool NeedNewPath()
    {
        return !hasPath;
    }
    public override void Navigate()
    {
        if (!movementAllowed || !isActiveNavigator || !hasPoint)
            return;

        agent.speed = originalSpeed;
        Vector3 p = point;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(p, out hit, 5f, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(hit.position, path);
            agent.path = path;
        }
    }
}
