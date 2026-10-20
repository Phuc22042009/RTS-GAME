using UnityEngine;

public class WalkState : IState
{
    private readonly Unit owner;
    private readonly StateMachine stateMachine;
    private readonly Unit target;

    public WalkState(Unit owner, StateMachine stateMachine, Unit target)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
        this.target = target;
    }

    public void Enter()
    {
    }

    public void Execute()
    {
        if (owner.Health != null && owner.Health.IsDead)
        {
            stateMachine.Transition(new DeathState(owner, stateMachine));
            return;
        }

        if (target == null || !owner.Perception.IsValidEnemy(target))
        {
            owner.Perception.ClearTarget();
            stateMachine.Transition(new IdleState(owner, stateMachine));
            return;
        }

        float distance = Vector2.Distance(owner.transform.position, target.transform.position);

        if (distance <= owner.Stats.attackRange)
        {
            owner.Movement.Stop();
            stateMachine.Transition(new AttackState(owner, stateMachine, target));
            return;
        }

        owner.Movement.MoveTowards(target.transform.position, owner.Stats.moveSpeed);
    }

    public void Exit()
    {
        if (owner.Movement != null)
        {
            owner.Movement.Stop();
        }
    }
}
