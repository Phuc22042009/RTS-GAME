using UnityEngine;

public class DeathState : IState
{
    private readonly Unit owner;
    private readonly StateMachine stateMachine;
    private float deathTimer;
    private const float CleanupDelay = 0.8f;

    public DeathState(Unit owner, StateMachine stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        deathTimer = CleanupDelay;

        if (owner.Movement != null)
        {
            owner.Movement.Stop();
        }

        Collider2D col = owner.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        UnitRegistry.Unregister(owner);
    }

    public void Execute()
    {
        deathTimer -= Time.deltaTime;
        if (deathTimer <= 0f)
        {
            if (owner != null && owner.gameObject != null)
            {
                owner.gameObject.SetActive(false);
            }
        }
    }

    public void Exit()
    {
    }
}
