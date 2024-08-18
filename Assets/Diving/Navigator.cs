using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
//Strategy pattern
// * what we want to navigate to
//  - //
// * what checks need to be satisfied in order to initiate the move
// * when we move
// * how we move along the path
//want to be able to put multiple navigators on an object and for the navigator system to be able to choose what it uses
//do random walk and a patrol path
public abstract class Navigator : MonoBehaviour, INavigator
{
    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] protected NavMeshAreas allowedAreas;
    [Disable] public Vector3 destination;
    protected NavMeshPathStatus pathStatus;
    [Disable] public bool hasPath;
    [Disable] public bool isOnNavMesh;
    [Disable] protected bool isPathStale;
    [Disable] protected bool isStopped;
    [Disable] public float remainingDistance;
    [Disable] public Vector3 velocity;
    [Disable] public float speed;
    protected virtual void Awake() { }
    protected virtual void Start() { 
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    [SerializeField] protected bool isActiveNavigator;
    [SerializeField] protected bool movementAllowed;
    protected virtual void Update()
    {
        if (!isActiveNavigator)
            return;
        if (!movementAllowed)
        {
            agent.velocity = Vector3.zero;
        }
        else
        {
            agent.velocity = agent.desiredVelocity;
        }

        destination = agent.destination;
        pathStatus = agent.pathStatus;
        hasPath = agent.hasPath;
        isOnNavMesh = agent.isOnNavMesh;
        isPathStale = agent.isPathStale;
        isStopped = agent.isStopped;
        remainingDistance = agent.remainingDistance;
        velocity = agent.velocity;
        speed = agent.velocity.magnitude;
    }
    public virtual bool NeedNewPath()
    {
        return !hasPath || agent.remainingDistance < agent.stoppingDistance;
    }
    public virtual void Navigate()
    {
    }
    public void MovementAllowed(bool value)
    {
        movementAllowed = value;
    }
    public void SetActiveNavigator(bool value)
    {
        isActiveNavigator = value;
    }
    public void SetSpeed(float value)
    {
        agent.speed = value;
    }
}
public interface INavigator
{
    public bool NeedNewPath();
    public void Navigate();
    public void MovementAllowed(bool value);
    public void SetActiveNavigator(bool value);
    public void SetSpeed(float value);
}