using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class QueueSystem : MonoBehaviour
{
    public static QueueSystem ins;
    private void Awake()
    {
        ins = this;
    }
    // Stores how many people are allowed in each row and how many columns are allowed 
    public int rowLength, columnLength;
    // Stores number of rows and columns taking account for transitions (snaking) between rows
    public int[,] queueLength;
    // Stores the position of the first place in queue
    public Transform queueStart;
    // Stores the distance that queue positions will be from each other
    public float queueSize;
    // Stores the max length of the queue
    [Disable] public int queueCapacity;
    // Stores the location of each position in the queue, where [0] is the start of the queue
    public Vector3[] queuePosition;

    public bool[] positionOccupied;

    public QueueDirection queueDirection;
    [Serializable] public enum QueueDirection
    {
        Left, Right
    }

    // Start is called before the first frame update
    void Start()
    {
        if (rowLength < 1 || columnLength < 1) throw new System.ArgumentException("Queue maxRows and maxColumns must be greater than 0.");

        queueStart = GetComponent<Transform>();
        queueCapacity = rowLength * columnLength + columnLength - 1;
        queuePosition = new Vector3[queueCapacity];

        queuePosition[0] = queueStart.position;

        //Creates room for queue to snake around to next line
        queueLength = new int[rowLength, columnLength * 2 - 1];

        // Tracks position in queue for mapping coordinates
        int position = 0;
        for (int column = 0; column < queueLength.GetLength(1); column++)
        {
            for (int row = 0; row < queueLength.GetLength(0); row++)
            {

                // If column complete and not at max columns, add transition column position
                if (column % 2 != 0 && column + 1 < queueLength.GetLength(1))
                {
                    column++;
                    queuePosition[position] = queuePosition[position - 1] + (queueSize * Vector3.down);
                    position++;
                }


                // Offset ensures row is built in the correct direction
                Vector3 offset = Vector3.zero;
                if(column % 4 == 0)
                {
                    // Set the Vector3 position of each place in the queue to its offset from the start
                    offset = row * queueSize * QueueDirectionToVector();
                } else
                {
                    offset = (queueLength.GetLength(0) - 1 - row) * queueSize * QueueDirectionToVector();
                }

                // Set the coordinates for current position
                queuePosition[position] = queueStart.position + offset + (column * queueSize * Vector3.down);

                position++;
            }
        }
        // All positions in queue are unoccupied
        positionOccupied = new bool[queueCapacity];
    }


    
    public Vector3 JoinQueue(int currentPositionInQueue, out int outPosition) // -1 is reserved for joining the queue from the outside
    {
        // If not in queue
        if(currentPositionInQueue == -1)
        {
            // Check queue positions from position 0 to queueCapacity-1 for first available free spot
            for (int i = 0; i < queueCapacity; i++)
            {
                if (!positionOccupied[i])
                {
                    positionOccupied[i] = true;
                    outPosition = i;
                    return queuePosition[i];
                }
            }
        }

        // If at front of queue
        if(currentPositionInQueue == 0)
        {
            // Check for free seat
            // Return seat position
            // Set positionOccupied[0] to false
            outPosition = 0;
            return queuePosition[0];

        }

        // If already in queue, check if you can move up
        if(currentPositionInQueue > 0 && currentPositionInQueue < queueCapacity)
        {
            // Check queue positions from position 0 to queueCapacity-1 for first available free spot
            for (int i = 0; i < currentPositionInQueue; i++)
            {
                if (!positionOccupied[i])
                {
                    positionOccupied[currentPositionInQueue] = false;
                    positionOccupied[i] = true;
                    outPosition = i;
                    return queuePosition[i];
                }
            }
            //Queue full, return error vector
            outPosition = currentPositionInQueue;
            return queuePosition[currentPositionInQueue];
        }
        outPosition = -1;
        return Vector3.zero;
    }





    Vector3 QueueDirectionToVector()
    {
        switch (queueDirection)
        {
            case QueueDirection.Left:
                return Vector3.left;
            case QueueDirection.Right:
                return Vector3.right;
        }
        return Vector3.zero;
    }

    void OnDrawGizmos()
    {
        if (queuePosition == null || queuePosition.Length == 0) return;

        Gizmos.color = Color.red;
        Handles.color = Color.green;

        int i = 0;
        foreach (Vector3 position in queuePosition)
        {
            Gizmos.DrawSphere(position, 0.1f); // Draw a sphere at each point
            Handles.Label(position + Vector3.up * 0.2f, i.ToString());
            i++;
        }
    }
}
