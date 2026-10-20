using UnityEngine;

public class AttackState : IState
{
    private readonly Unit owner;
    private readonly StateMachine stateMachine;
    private readonly Unit target;
    private const float RangeBuffer = 0.25f;

    public AttackState(Unit owner, StateMachine stateMachine, Unit target)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
        this.target = target;
    }

    public void Enter()
    {
        if (owner.Movement != null)
        {
            owner.Movement.Stop();
            if (target != null)
            {
                owner.Movement.RotateTowards(target.transform.position);
            }
        }
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

        owner.Movement.RotateTowards(target.transform.position);

        float distance = Vector2.Distance(owner.transform.position, target.transform.position);

        if (distance > owner.Stats.attackRange + RangeBuffer)
        {
            stateMachine.Transition(new WalkState(owner, stateMachine, target));
            return;
        }

        if (owner.Combat.CanAttack())
        {
            if (owner.Combat.CanCastAbility())
            {
                stateMachine.Transition(new CastAbilityState(owner, stateMachine, target));
            }
            else
            {
                owner.Combat.PerformAttack(target);
            }
        }
    }

    public void Exit()
    {
    }
}
