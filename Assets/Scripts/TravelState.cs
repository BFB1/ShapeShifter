using System;
using UnityEngine;

public class TravelState : BaseState
{
    private const float Delta = 0.01f;
    private readonly Airplane airplane;


    public TravelState(Airplane airplane) : base(airplane.gameObject)
    {
        this.airplane = airplane;
    }

    public override Type Tick()
    {
        if (airplane.Destination == null || airplane.Destination == airplane.CurrentStation)
        {
            return typeof(IdleState);
        }
        Vector3 destination = airplane.Destination.transform.position;
        
        if (Vector3.Distance(airplane.transform.position, destination) <= Delta)
        {
            airplane.CurrentStation = airplane.Destination;
            return typeof(StationState);
        }

        airplane.transform.Translate((destination - airplane.transform.position).normalized * 0.01f);
        return null;
    }
}
