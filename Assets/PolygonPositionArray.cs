using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class PolygonPositionArray
{
    [SerializeField, ReorderableList] List<Transform> points = new();
    int totalSpaces;
    [Range(0.0025f, 1f)] public float spacesDistance;
    public Vector2[] positions;
    public PolygonPositionArray(List<Transform> points)
    {
        this.points = points;
    }
    public void RecomputePositions()
    {
        if (points.Count < 2)
        {
            positions = null;
            return;
        }
        float totalDistance = 0f;
        for (int i = 0; i < points.Count - 1; i++)
        {
            totalDistance += Vector2.Distance(points[i].position, points[i + 1].position);
        }
        totalSpaces = Mathf.RoundToInt(totalDistance / spacesDistance);
        positions = new Vector2[totalSpaces];
        for (int i = 0; i < totalSpaces; i++)
        {
            Vector2 pos;
            if (TryGetPositionInQueue_Distance(i, out pos))
            {
                positions[i] = pos;
            }
        }
    }
    public bool TryGetPositionInQueue_Distance(int posNumber, out Vector2 pos)
    {
        pos = Vector2.zero;

        if (points.Count < 2)
            return false;

        float queueDistance = (float)posNumber * spacesDistance;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 waypointPosA = points[i].position;
            Vector2 waypointPosB = points[i + 1].position;
            float distAB = Vector2.Distance(waypointPosA, waypointPosB);

            if (queueDistance > distAB)
            {
                queueDistance -= distAB;
                continue;
            }
            else
            {
                float lerpValue = Mathf.InverseLerp(0f, distAB, queueDistance);
                pos = Vector2.Lerp(waypointPosA, waypointPosB, lerpValue);
                break;
            }
        }
        return true;
    }
    public bool TryGetPositionAtIndex(int index, out Vector2 pos)
    {
        pos = Vector2.zero;
        if (positions == null || index >= positions.Length)
            return false;

        pos = positions[index];

        return true;
    }
}
