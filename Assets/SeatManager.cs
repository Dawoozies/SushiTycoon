using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public static SeatManager ins;
    private void Awake()
    {
        ins = this;
    }

    [ReorderableList] public Seat[] seats;
    [ReorderableList] public Seat[] availableSeats;

    // Start is called before the first frame update
    void Start()
    {
        seats = GetComponentsInChildren<Seat>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Seat seat in seats)
        {
            if (!seat.isDirty && !seat.isOccupied)
            {

            }
        }
    }

    public Seat RandomAvailableSeat()
    {
        if (seats.Length == 0) return null;
        List<Seat> availableSeats = new List<Seat>();
        foreach (Seat seat in seats)
        {
            if (!seat.isDirty && !seat.isOccupied)
            {
                availableSeats.Add(seat);
            }
        }
        if (availableSeats.Count == 0) return null;
        int randomIndex = Random.Range(0, availableSeats.Count);
        availableSeats[randomIndex].isOccupied = true;
        return availableSeats[randomIndex];
    }
}
