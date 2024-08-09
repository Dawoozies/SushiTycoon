using System.Collections;
using System.Collections.Generic;
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
    protected NavMeshAgent agent;
    public Vector3 destination;
    protected NavMeshPathStatus pathStatus;
    public bool hasPath;
    public bool isOnNavMesh;
    protected bool isPathStale;
    protected bool isStopped;
    public float remainingDistance;
    public Vector3 velocity;
    public float speed;
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
}
public interface INavigator
{
    public bool NeedNewPath();
    public void Navigate();
    public void MovementAllowed(bool value);
    public void SetActiveNavigator(bool value);
}