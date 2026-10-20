using UnityEngine;

public interface IDamageable
{
    void TakeDamage(DamageInfo damageInfo);
    bool IsDead { get; }
    GameObject gameObject { get; }
    Transform transform { get; }
}
