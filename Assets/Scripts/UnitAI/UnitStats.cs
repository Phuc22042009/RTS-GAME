using System;
using UnityEngine;

[System.Serializable]
public class UnitStats
{
    [Header("Identity & Team")]
    public int TeamID;
    public string unitName = "Champion";

    [Header("Health & Durability")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Combat")]
    public float attackDamage = 15f;
    public float attackRange = 1.5f;
    public float attackSpeed = 0.8f;
    public bool isRanged = false;
    public float projectileSpeed = 8f;

    [Header("Mana & Abilities")]
    public float startingMana = 0f;
    public float currentMana;
    public float maxMana = 100f;
    public float manaPerAttack = 15f;
    public float manaPerDamageTaken = 10f;
    public float abilityDamageMultiplier = 2.5f;

    [Header("Locomotion")]
    public float moveSpeed = 3.5f;

    public void Initialize()
    {
        if (maxHealth <= 0f) maxHealth = 100f;
        currentHealth = maxHealth;

        if (attackDamage <= 0f) attackDamage = 15f;
        if (attackRange < 1.5f) attackRange = 1.5f;
        if (attackSpeed <= 0f) attackSpeed = 0.8f;
        if (moveSpeed <= 0f) moveSpeed = 3.5f;

        if (maxMana <= 0f) maxMana = 100f;
        if (manaPerAttack <= 0f) manaPerAttack = 15f;
        if (manaPerDamageTaken <= 0f) manaPerDamageTaken = 10f;
        if (abilityDamageMultiplier <= 0f) abilityDamageMultiplier = 2.5f;
        if (projectileSpeed <= 0f) projectileSpeed = 8f;

        currentMana = Mathf.Clamp(startingMana, 0f, maxMana);
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0f);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void AddMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }

    public void ResetMana()
    {
        currentMana = 0f;
    }

    public bool IsDead()
    {
        return currentHealth <= 0f;
    }
}
