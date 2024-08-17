using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaitingArea : MonoBehaviour
{
    public static WaitingArea ins;
    private void Awake()
    {
        ins = this;
    }
    [Disable] public int totalSpaces;
    public int spacesPerInterval;
    int _spacesPerInterval;
    [Range(0.025f, 1f)] public float spacesDistance;
    float _spacesDistance;
    public List<GameObject> waypoints = new();
    Vector2[] queuePositions;
    List<object> inWaitingArea = new();
    public bool canQueue => queuePositions != null && queuePositions.Length > 0;
    public QueueCalculationType queueCalculationType;
    public enum QueueCalculationType
    {
        SetByDistanceBetweenSpaces, SetBySpacesPerInterval
    }
    public void OnObjectBuildHandler(GameObject builtObject)
    {
        waypoints.Add(builtObject);
        RecomputePositions();
    }
    public void OnObjectDeletedHandler(GameObject deletedObject)
    {
        if(waypoints.Contains(deletedObject))
            waypoints.Remove(deletedObject);
        RecomputePositions();
    }
    public bool TryEnqueue(object toEnterQueue, out Func<object, QueueData> fetchQueuePosition)
    {
        fetchQueuePosition = null;
        if (inWaitingArea.Count >= totalSpaces)
            return false;

        inWaitingArea.Add(toEnterQueue);
        fetchQueuePosition = FetchQueuePosition;
        return true;
    }
    public void LeaveQueue(object toLeaveQueue)
    {
        //indexless operations because of the Func!!!
        if(inWaitingArea.Contains(toLeaveQueue))
            inWaitingArea.Remove(toLeaveQueue);
    }
    QueueData outgoingData;
    QueueData FetchQueuePosition(object objectInQueue)
    {
        int queueNumber = inWaitingArea.IndexOf(objectInQueue);
        if(queuePositions == null || queueNumber >= queuePositions.Length)
        {
            outgoingData.queueNumber = -1;
            return outgoingData;
        }
        outgoingData.position = queuePositions[queueNumber];
        outgoingData.queueNumber = queueNumber;
        return outgoingData;
    }
    private void Update()
    {
        if(_spacesDistance != spacesDistance)
        {
            _spacesDistance = spacesDistance;
            RecomputePositions();
        }
        if(_spacesPerInterval != spacesPerInterval)
        {
            _spacesPerInterval = spacesPerInterval;
            RecomputePositions();
        }
    }
    void RecomputePositions()
    {
        if (waypoints.Count < 2)
        {
            queuePositions = null;
            return;
        }
        switch (queueCalculationType)
        {
            case QueueCalculationType.SetByDistanceBetweenSpaces:
                float totalDistance = 0f;
                for (int i = 0; i < waypoints.Count - 1; i++)
                {
                    totalDistance += Vector2.Distance(waypoints[i].transform.position, waypoints[i + 1].transform.position);
                }
                totalSpaces = Mathf.RoundToInt(totalDistance / spacesDistance);
                queuePositions = new Vector2[totalSpaces];
                for (int i = 0; i < totalSpaces; i++)
                {
                    Vector2 pos;
                    if (TryGetPositionInQueue_Distance(i, out pos))
                    {
                        queuePositions[i] = pos;
                    }
                }
                break;
            case QueueCalculationType.SetBySpacesPerInterval:
                totalSpaces = spacesPerInterval * (waypoints.Count - 1);
                queuePositions = new Vector2[totalSpaces];
                for (int i = 0; i < totalSpaces; i++)
                {
                    Vector2 pos;
                    if (TryGetPositionInQueue(i, out pos))
                    {
                        queuePositions[i] = pos;
                    }
                }
                break;
        }
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        if (waypoints.Count < 2)
            return;
        if (queuePositions == null || queuePositions.Length == 0)
            return;
        Gizmos.color = Color.cyan;
        foreach (Vector2 pos in queuePositions)
        {
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }
    public bool TryGetPositionInQueue(int queueNumber, out Vector2 queuePosition)
    {
        queuePosition = Vector2.zero;
        int spacesTotal = 0;
        if (waypoints.Count < 2)
            return false;

        spacesTotal = (waypoints.Count - 1)*spacesPerInterval;
        if (queueNumber > spacesTotal)
            return false;

        if (spacesPerInterval <= 0)
            return false;

        int waypoint = -1;
        int i = queueNumber;
        while(i >= 0)
        {
            waypoint++;
            i -= spacesPerInterval;
            if (i < 0)
                break;
        }

        float lerpValue = Mathf.InverseLerp(0f, (float)spacesPerInterval, (float)(queueNumber % spacesPerInterval));
        queuePosition = Vector2.Lerp(waypoints[waypoint].transform.position, waypoints[waypoint+1].transform.position, lerpValue);
        return true;
    }
    public bool TryGetPositionInQueue_Distance(int queueNumber, out Vector2 queuePosition)
    {
        queuePosition = Vector2.zero;

        if (waypoints.Count < 2)
            return false;

        float queueDistance = (float)queueNumber * spacesDistance;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector2 waypointPosA = waypoints[i].transform.position;
            Vector2 waypointPosB = waypoints[i + 1].transform.position;
            float distAB = Vector2.Distance(waypointPosA, waypointPosB);

            if(queueDistance > distAB)
            {
                queueDistance -= distAB;
                continue;
            }
            else
            {
                float lerpValue = Mathf.InverseLerp(0f, distAB, queueDistance);
                queuePosition = Vector2.Lerp(waypointPosA, waypointPosB, lerpValue);
                break;
            }
        }

        return true;
    }
}
public struct QueueData
{
    public Vector2 position;
    public int queueNumber;
}