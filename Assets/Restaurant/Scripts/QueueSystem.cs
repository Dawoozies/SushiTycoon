using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class QueueSystem : MonoBehaviour
{

    public int maxRows, maxColumns;
    public int[,] queueLength;
    public Transform queueStart;
    public float queueSize;
    //public int queueCount;
    public int queueCapacity;
    public Vector3[] queuePosition;

    public QueueDirection queueDirection;
    [Serializable] public enum QueueDirection
    {
        Left, Right
    }

    // Start is called before the first frame update
    void Start()
    {
        if (maxRows < 1 || maxColumns < 1) throw new System.ArgumentException("Queue maxRows and maxColumns must be greater than 0.");

        queueCapacity = maxRows * maxColumns + maxColumns - 1;
        queuePosition = new Vector3[queueCapacity];

        queuePosition[0] = queueStart.position;

        //Creates room for queue to snake around to next line
        queueLength = new int[maxRows, maxColumns * 2 - 1];
        Debug.Log("Length 0 " + queueLength.GetLength(0));
        Debug.Log(queueLength.GetLength(1));


        int position = 0;
        for (int column = 0; column < queueLength.GetLength(1); column++)
        {
            for (int row = 0; row < queueLength.GetLength(0); row++)
            {

                if (column % 2 != 0 && column + 1 < queueLength.GetLength(1))
                {
                    //position++;
                    column++;
                    Debug.Log("Increment column");
                    queuePosition[position] = queuePosition[position - 1] + (queueSize * Vector3.down);
                    position++;
                }

                Vector3 offset = Vector3.zero;
                if(column % 4 == 0)
                {
                    // Set the Vector3 position of each place in the queue to its offset from the start
                    offset = row * queueSize * QueueDirectionToVector();
                } else
                {
                    offset = (queueLength.GetLength(0) - 1 - row) * queueSize * QueueDirectionToVector();
                }
                queuePosition[position] = queueStart.position + offset + (column * queueSize * Vector3.down);
                Debug.Log(queuePosition[position]);
                Debug.Log("Position: " + position);
                position++;
            }

            // If column complete and not at max columns, add transition column place

            Debug.Log("Position: " + position);
        }



        // Create the queue. adds extra positions to connect the rows (snaking)
       /* position = 0;
        while (position < queueCapacity)
        {
            //queuePosition[position] = queueStart.position + new Vector3()            
            position++;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
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
