using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Seat : MonoBehaviour
{
    public SeatingDirection seatingDirection;
    public bool isOccupied;
    public bool isDirty;
    Vector2 seatDirection;

    [Serializable] public enum SeatingDirection
    {
        Left, Right, Up, Down
    }

    // Start is called before the first frame update
    void Start()
    {
        isOccupied = false;
        isDirty = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + SeatingDirectionToVector() * SeatingParameters.ins.SeatingDistance);
    }
    Vector3 SeatingDirectionToVector()
    {
        switch (seatingDirection)
        {
            case SeatingDirection.Left:
                return Vector3.left;
            case SeatingDirection.Right:
                return Vector3.right;
            case SeatingDirection.Up:
                return Vector3.up;
            case SeatingDirection.Down:
                return Vector3.down;
        }
        return Vector3.zero;
    }

}
