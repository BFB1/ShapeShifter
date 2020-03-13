using System;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    private List<int> passengers;
    private StateMachine sm;

    private List<Station> path;

    [NonSerialized] public const int Capacity = 20;

    public Station Destination { get; private set; }
    public Station CurrentStation { get; set; }

    public List<int> Passengers => passengers;

    public bool AddPassenger(int shape)
    {
        if (passengers.Count >= Capacity) return false;
        passengers.Add(shape);
        return true;

    }
    public void RemovePassengers(int shape)
    {
        passengers.RemoveAll((item) => (item == shape) );
    }

    public void NewDestination(Station newDestination)
    {
        Destination = newDestination;
    }

    private void Awake()
    {
        passengers = new List<int>();
        path = new List<Station>();
        
        /*
         * Create a new StateMachine for this Airplane.
         * 
         * Both the StateMachine and the states are only created once
         * and persists until this object gets deleted or goes to sleep
         */
        sm = new StateMachine(new Dictionary<Type, BaseState>()
        {
            {typeof(TravelState), new TravelState(this)},
            {typeof(IdleState), new IdleState(this)},
            {typeof(StationState), new StationState(this)}
        });
    }

    private void Update()
    {
        // The only thing this object does every frame is updating the StateMachine.
        sm.StateUpdate();
    }
}
