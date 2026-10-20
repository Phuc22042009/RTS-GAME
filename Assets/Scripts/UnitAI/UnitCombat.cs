using System;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    private Unit owner;
    private float attackCooldownTimer;

    public event Action<Unit> OnAttackStarted;
    public event Action<Unit, DamageInfo> OnAttackHit;
    public event Action<float, float> OnManaChanged;
    public event Action<Unit> OnAbilityCast;

    public void Initialize(Unit owner)
    {
        this.owner = owner;
        attackCooldownTimer = 0f;

        OnManaChanged?.Invoke(owner.Stats.currentMana, owner.Stats.maxMana);
    }

    private void Update()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    public bool CanAttack()
    {
        return attackCooldownTimer <= 0f;
    }

    public bool CanCastAbility()
    {
        return owner != null && owner.Stats != null && owner.Stats.currentMana >= owner.Stats.maxMana;
    }

    public void PerformAttack(Unit target)
    {
        if (target == null || target.Health.IsDead)
            return;

        float cooldown = 1f / Mathf.Max(owner.Stats.attackSpeed, 0.1f);
        attackCooldownTimer = cooldown;

        OnAttackStarted?.Invoke(target);

        GainMana(owner.Stats.manaPerAttack);

        DamageInfo damageInfo = new DamageInfo(owner.Stats.attackDamage, owner.gameObject, false);

        if (owner.Stats.isRanged)
        {
            FireProjectile(target, damageInfo);
        }
        else
        {
            ExecuteMeleeHit(target, damageInfo);
        }
    }

    public void CastAbility(Unit target)
    {
        if (target == null || target.Health.IsDead)
            return;

        float cooldown = 1f / Mathf.Max(owner.Stats.attackSpeed, 0.1f);
        attackCooldownTimer = cooldown;

        owner.Stats.ResetMana();
        OnManaChanged?.Invoke(owner.Stats.currentMana, owner.Stats.maxMana);

        OnAbilityCast?.Invoke(target);

        float abilityDamage = owner.Stats.attackDamage * owner.Stats.abilityDamageMultiplier;
        DamageInfo critDamageInfo = new DamageInfo(abilityDamage, owner.gameObject, true);

        if (owner.Stats.isRanged)
        {
            FireProjectile(target, critDamageInfo);
        }
        else
        {
            ExecuteMeleeHit(target, critDamageInfo);
        }
    }

    public void GainMana(float amount)
    {
        if (owner == null || owner.Stats == null)
            return;

        owner.Stats.AddMana(amount);
        OnManaChanged?.Invoke(owner.Stats.currentMana, owner.Stats.maxMana);
    }

    private void FireProjectile(Unit target, DamageInfo damageInfo)
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        GameObject projObj = null;
        if (projectilePrefab != null)
        {
            projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        }

        if (projObj != null)
        {
            ProjectileScript projectile = projObj.GetComponent<ProjectileScript>();
            if (projectile != null)
            {
                projectile.Initialize(damageInfo, target, owner.Stats.projectileSpeed);
            }
        }
        else
        {
            ExecuteMeleeHit(target, damageInfo);
        }
    }

    private void ExecuteMeleeHit(Unit target, DamageInfo damageInfo)
    {
        if (target != null && target.Health != null)
        {
            target.Health.TakeDamage(damageInfo);
            OnAttackHit?.Invoke(target, damageInfo);
        }
    }

    public void SetProjectilePrefab(GameObject prefab)
    {
        projectilePrefab = prefab;
    }
}
