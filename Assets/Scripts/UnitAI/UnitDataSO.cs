using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "RTS/Unit Data")]
public class UnitDataSO : ScriptableObject
{
    [Header("Identity")]
    public string unitName = "Champion";

    [Header("Vitality & Mobility")]
    public float maxHealth = 100f;
    public float moveSpeed = 3.5f;

    [Header("Combat")]
    public float attackDamage = 15f;
    public float attackRange = 1.5f;
    public float attackSpeed = 0.8f;
    public bool isRanged = false;
    public float projectileSpeed = 8f;

    [Header("Mana & Ability")]
    public float startingMana = 0f;
    public float maxMana = 100f;
    public float manaPerAttack = 15f;
    public float manaPerDamageTaken = 10f;
    public float abilityDamageMultiplier = 2.5f;

    [Header("Visuals & Prefabs")]
    public Sprite unitSprite;
    public GameObject projectilePrefab;
    public GameObject deathVfxPrefab;

    public void ApplyToStats(UnitStats targetStats)
    {
        if (targetStats == null) return;

        targetStats.unitName = unitName;
        targetStats.maxHealth = maxHealth;
        targetStats.currentHealth = maxHealth;
        targetStats.moveSpeed = moveSpeed;
        targetStats.attackDamage = attackDamage;
        targetStats.attackRange = attackRange;
        targetStats.attackSpeed = attackSpeed;
        targetStats.isRanged = isRanged;
        targetStats.projectileSpeed = projectileSpeed;
        targetStats.startingMana = startingMana;
        targetStats.currentMana = startingMana;
        targetStats.maxMana = maxMana;
        targetStats.manaPerAttack = manaPerAttack;
        targetStats.manaPerDamageTaken = manaPerDamageTaken;
        targetStats.abilityDamageMultiplier = abilityDamageMultiplier;
    }
}
