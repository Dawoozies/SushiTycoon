using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public static SeatManager ins;
    private void Awake()
    {
        ins = this;
    }

    [ReorderableList] public List<Seat> seats = new();
    [ReorderableList] public List<Seat> availableSeats = new();
    [ReorderableList] public List<Seat> dirtySeats= new();
    [ReorderableList] public List<Seat> unwaitedSeats = new();


    // Start is called before the first frame update
    void Start()
    {
        seats = GetComponentsInChildren<Seat>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Seat seat in seats)
        {
            if (seat.isDirty)
            {
                //seat.G
            }
        }
    }

    public Seat RandomAvailableSeat()
    {
        if (seats.Count == 0) return null;
        availableSeats.Clear();


        foreach (Seat seat in seats)
        {
            if (seat.isDirty || seat.isOccupied)
            {
                continue;
            }
            availableSeats.Add(seat);
        }
        if (availableSeats.Count == 0) return null;
        int randomIndex = Random.Range(0, availableSeats.Count);
        availableSeats[randomIndex].isOccupied = true;
        return availableSeats[randomIndex];
    }

    public Seat RandomSeatWaiterNeeded()
    {
        if (seats.Count == 0) return null;

        unwaitedSeats.Clear();

        foreach (Seat seat in seats)
        {
            if (seat.waiterNeeded)
            {
                 unwaitedSeats.Add(seat);
            }
        }
        if (unwaitedSeats.Count == 0) return null;

        return unwaitedSeats[Random.Range(0, unwaitedSeats.Count)];
    }

    public void AddToDirtySeats(Seat seat)
    {
        dirtySeats.Add(seat);
    }



    public Seat GetDirtySeat()
    {
        if (dirtySeats.Count == 0) return null;
        Seat dirtySeat = dirtySeats[0];
        dirtySeats.RemoveAt(0);
        return dirtySeat;
    }
}
