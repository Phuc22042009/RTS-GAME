using UnityEngine;

public class StunnedState : IState
{
    private readonly Unit owner;
    private readonly StateMachine stateMachine;
    private float stunTimer;

    public StunnedState(Unit owner, StateMachine stateMachine, float duration)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
        this.stunTimer = duration;
    }

    public void Enter()
    {
        if (owner.Movement != null)
        {
            owner.Movement.Stop();
        }
    }

    public void Execute()
    {
        if (owner.Health != null && owner.Health.IsDead)
        {
            stateMachine.Transition(new DeathState(owner, stateMachine));
            return;
        }

        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            stateMachine.Transition(new IdleState(owner, stateMachine));
        }
    }

    public void Exit()
    {
    }
}
