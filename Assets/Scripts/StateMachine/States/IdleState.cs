using UnityEngine;

public class IdleState : IState
{
    private readonly Unit owner;
    private readonly StateMachine stateMachine;

    public IdleState(Unit owner, StateMachine stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
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

        Unit target = owner.Perception.FindNearestEnemy();
        if (target == null)
        {
            return;
        }

        float distance = Vector2.Distance(owner.transform.position, target.transform.position);

        if (distance <= owner.Stats.attackRange)
        {
            stateMachine.Transition(new AttackState(owner, stateMachine, target));
        }
        else
        {
            stateMachine.Transition(new WalkState(owner, stateMachine, target));
        }
    }

    public void Exit()
    {
    }
}
