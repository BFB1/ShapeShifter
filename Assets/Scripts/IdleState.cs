using System;
using UnityEngine;

public class IdleState : BaseState
{
    private Airplane airplane;


    public IdleState(Airplane airplane) : base(airplane.gameObject)
    {
        this.airplane = airplane;
    }

    public override Type Tick()
    {
        Debug.Log("IdleState Active");
        return null;
    }
}
