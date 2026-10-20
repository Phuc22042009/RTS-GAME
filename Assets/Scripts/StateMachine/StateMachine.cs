using System;

public class StateMachine
{
    public IState CurrentState { get; private set; }

    public event Action<IState, IState> OnStateChanged;

    public void Initialize(IState initialState)
    {
        CurrentState = initialState;
        CurrentState?.Enter();
        OnStateChanged?.Invoke(null, CurrentState);
    }

    public void Transition(IState nextState)
    {
        if (nextState == null || CurrentState == nextState)
            return;

        IState previousState = CurrentState;
        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState?.Enter();

        OnStateChanged?.Invoke(previousState, CurrentState);
    }

    public void Execute()
    {
        CurrentState?.Execute();
    }
}
