using System;

public abstract class BaseState<EState>
    where EState : Enum
{
    public BaseState(EState key)
    {
        stateKey = key;
    }

    public EState stateKey { get; private set; }

    /// <summary>
    /// once on start
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// every frame
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// once on exit
    /// </summary>
    public abstract void ExitState();

    /// <summary>
    /// returns what state will be updated next
    /// </summary>
    public abstract EState GetNextState();
}
