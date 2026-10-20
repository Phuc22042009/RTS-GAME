using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth = 100f;

    private Unit owner;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0f;

    public event Action<float, float> OnHealthChanged;
    public event Action<DamageInfo> OnTakeDamage;
    public event Action<Unit> OnDeath;

    public void Initialize(Unit owner, float initialMaxHealth)
    {
        this.owner = owner;
        maxHealth = Mathf.Max(initialMaxHealth, 1f);
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead)
            return;

        currentHealth -= damageInfo.amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

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
