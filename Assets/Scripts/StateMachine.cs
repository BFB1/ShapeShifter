using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StateMachine
{
    private Dictionary<Type, BaseState> availableStates;
    public BaseState CurrentState { get; private set; }
    public event Action<BaseState> OnStateChanged;

    public StateMachine(Dictionary<Type, BaseState> availableStates)
    {
        this.availableStates = availableStates;
    }

    public void StateUpdate()
    {
        if (CurrentState == null)
        {
            CurrentState = availableStates.Values.First();
        }

        Type nextState = CurrentState.Tick();

        if (nextState != null && nextState != CurrentState.GetType())
        {
            SwitchToNewState(nextState);
        }
    }

    public void SwitchToNewState(Type newState)
    {
        CurrentState = availableStates[newState];
        OnStateChanged?.Invoke(CurrentState);
    }
}
