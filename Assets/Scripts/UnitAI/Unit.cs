using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitDataSO unitData;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private UnitStats stats = new UnitStats();

    private UnitMovement movement;
    private UnitPerception perception;
    private UnitCombat combat;
    private HealthSystem health;
    private UnitVisual visual;
    private StateMachine stateMachine;

    public StateMachine StateMachine => stateMachine;
    public UnitStats Stats => stats;
    public UnitDataSO UnitData => unitData;
    public UnitPerception Perception => perception;
    public UnitMovement Movement => movement;
    public UnitMovement UnitMovement => movement;
    public UnitCombat Combat => combat;
    public HealthSystem Health => health;
    public UnitVisual Visual => visual;
    public bool IsDead => health != null && health.IsDead;

    private void Awake()
    {
        if (unitData != null)
        {
            unitData.ApplyToStats(stats);
        }
        else if (moveSpeed > 0f)
        {
            stats.moveSpeed = moveSpeed;
        }
        stats.Initialize();

        movement = GetComponent<UnitMovement>();
        if (movement == null)
        {
            movement = gameObject.AddComponent<UnitMovement>();
        }
        movement.Initialize(this);

        health = GetComponent<HealthSystem>();
        if (health == null)
        {
            health = gameObject.AddComponent<HealthSystem>();
        }
        health.Initialize(this, stats.maxHealth);

        combat = GetComponent<UnitCombat>();
        if (combat == null)
        {
            combat = gameObject.AddComponent<UnitCombat>();
        }
        combat.Initialize(this);

        visual = GetComponent<UnitVisual>();
        if (visual == null)
        {
            visual = gameObject.AddComponent<UnitVisual>();
        }
        visual.Initialize(this);

        perception = new UnitPerception(this);

        stateMachine = new StateMachine();
        stateMachine.Initialize(new IdleState(this, stateMachine));

        health.OnDeath += HandleDeathEvent;
    }

    private void Update()
    {
        stateMachine.Execute();
    }

    private void OnEnable()
    {
        UnitRegistry.Register(this);
    }

    private void OnDisable()
    {
        UnitRegistry.Unregister(this);
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeathEvent;
        }
    }

    private void HandleDeathEvent(Unit dyingUnit)
    {
        if (stateMachine != null && !(stateMachine.CurrentState is DeathState))
        {
            stateMachine.Transition(new DeathState(this, stateMachine));
        }
    }

    public void Move(Unit target)
    {
        if (target != null && movement != null)
        {
            movement.MoveTowards(target.transform.position, stats.moveSpeed);
        }
    }

    public void ResetUnit(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        gameObject.SetActive(true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        if (unitData != null)
        {
            unitData.ApplyToStats(stats);
        }
        stats.Initialize();
        if (health != null)
        {
            health.Initialize(this, stats.maxHealth);
        }
        if (combat != null)
        {
            combat.Initialize(this);
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        UnitRegistry.Register(this);

        stateMachine = new StateMachine();
        stateMachine.Initialize(new IdleState(this, stateMachine));
    }
}
