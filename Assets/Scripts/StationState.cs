using System;
using System.Collections.Generic;
using System.Linq;

public class StationState : BaseState
{
    private readonly Airplane airplane;
    
    public StationState(Airplane airplane) : base(airplane.gameObject)
    {
        this.airplane = airplane;
    }

    public override Type Tick()
    {
        if (airplane.CurrentStation == null)
        {
            return typeof(IdleState);
        }

        airplane.RemovePassengers(airplane.CurrentStation.Shape);
            
        List<int> takenPassengers = airplane.CurrentStation.queue.TakeWhile(passenger => airplane.AddPassenger(passenger)).ToList();
        foreach (int takenPassenger in takenPassengers)
        {
            airplane.CurrentStation.queue.Remove(takenPassenger);
        }

        if (airplane.CurrentStation != airplane.Destination)
        {
            airplane.CurrentStation = null;
            return typeof(TravelState);
        }
        else
        {
            return null;
        }
    }
}
