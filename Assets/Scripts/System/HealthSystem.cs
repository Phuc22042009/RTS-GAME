using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentShield = 0f;

    private Unit owner;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentShield => currentShield;
    public bool IsDead => currentHealth <= 0f;

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnShieldChanged;
    public event Action<DamageInfo> OnTakeDamage;
    public event Action<Unit> OnDeath;

    public void Initialize(Unit owner, float initialMaxHealth)
    {
        this.owner = owner;
        maxHealth = Mathf.Max(initialMaxHealth, 1f);
        currentHealth = maxHealth;
        currentShield = 0f;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnShieldChanged?.Invoke(currentShield);
    }

    public void AddShield(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        currentShield += amount;
        OnShieldChanged?.Invoke(currentShield);
    }

    public void Stun(float duration)
    {
        if (IsDead || duration <= 0f)
            return;

        if (owner != null && owner.StateMachine != null)
        {
            owner.StateMachine.Transition(new StunnedState(owner, owner.StateMachine, duration));
        }
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead)
            return;

        float incomingDamage = damageInfo.amount;

        if (currentShield > 0f)
        {
            if (currentShield >= incomingDamage)
            {
                currentShield -= incomingDamage;
                incomingDamage = 0f;
            }
            else
            {
                incomingDamage -= currentShield;
                currentShield = 0f;
            }
            OnShieldChanged?.Invoke(currentShield);
        }

        if (incomingDamage > 0f)
        {
            currentHealth -= incomingDamage;
            currentHealth = Mathf.Max(currentHealth, 0f);
        }

        if (owner != null && owner.Stats != null)
        {
            owner.Stats.currentHealth = currentHealth;
        }

        if (owner != null && owner.Combat != null && !IsDead)
        {
            owner.Combat.GainMana(owner.Stats.manaPerDamageTaken);
        }

        OnTakeDamage?.Invoke(damageInfo);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (owner != null && owner.Stats != null)
        {
            owner.Stats.currentHealth = currentHealth;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke(owner);
    }
}
