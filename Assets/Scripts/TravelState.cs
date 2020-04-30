using System;
using UnityEngine;

public class TravelState : BaseState
{
    private const float Delta = 0.01f;
    private readonly Airplane airplane;
    private readonly Transform airplaneTransform;


    public TravelState(Airplane airplane) : base(airplane.gameObject)
    {
        this.airplane = airplane;
        airplaneTransform = airplane.transform;
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

        Vector3 difference = destination - airplane.transform.position;
        
        airplaneTransform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90);
        airplaneTransform.position += (difference).normalized * 0.01f;
        return null;
    }
}
