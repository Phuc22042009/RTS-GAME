using UnityEngine;

public class CastAbilityState : IState
{
    private readonly Unit owner;
    private readonly StateMachine stateMachine;
    private readonly Unit target;
    private float castTimer;
    private const float CastDuration = 0.35f;

    public CastAbilityState(Unit owner, StateMachine stateMachine, Unit target)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
        this.target = target;
    }

    public void Enter()
    {
        castTimer = CastDuration;

        if (owner.Movement != null)
        {
            owner.Movement.Stop();
            if (target != null)
            {
                owner.Movement.RotateTowards(target.transform.position);
            }
        }

        if (owner.Combat != null && target != null)
        {
            owner.Combat.CastAbility(target);
        }
    }

    public void Execute()
    {
        if (owner.Health != null && owner.Health.IsDead)
        {
            stateMachine.Transition(new DeathState(owner, stateMachine));
            return;
        }

        castTimer -= Time.deltaTime;
        if (castTimer <= 0f)
        {
            if (target != null && owner.Perception.IsValidEnemy(target))
            {
                stateMachine.Transition(new AttackState(owner, stateMachine, target));
            }
            else
            {
                stateMachine.Transition(new IdleState(owner, stateMachine));
            }
        }
    }

    public void Exit()
    {
    }
}
