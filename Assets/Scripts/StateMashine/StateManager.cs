using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour
    where EState : Enum
{
    [SerializeField, Header("For debugging purposes only")]
    EState unityInspectorState;
    protected Dictionary<EState, BaseState<EState>> States =
        new Dictionary<EState, BaseState<EState>>();
    protected BaseState<EState> currentState;
    protected bool isTransitioningState = false;

    void Start()
    {
        currentState.EnterState();
    }

    void FixedUpdate()
    {
        unityInspectorState = currentState.stateKey;

        EState nextState = currentState.GetNextState();
        #region Handle state updating and transitioning
        if (isTransitioningState)
            return;

        if (nextState.Equals(currentState.stateKey))
            currentState.UpdateState();
        else
            TransitionToState(nextState);
        #endregion
    }

    public void TransitionToState(EState stateKey)
    {
        isTransitioningState = true;
        currentState.ExitState();
        currentState = States[stateKey];
        currentState.EnterState();
        isTransitioningState = false;
    }

    public EState GetCurrentState()
    {
        return currentState.stateKey;
    }
}
