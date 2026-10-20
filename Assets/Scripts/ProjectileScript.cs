using System.Collections;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] public GameObject hitEffect;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 4f;

    private DamageInfo damageInfo;
    private Unit targetUnit;
    private Vector2 lastTargetPos;
    private bool hasInitialized = false;

    public void Initialize(DamageInfo damageInfo, Unit target, float speed = 10f)
    {
        this.damageInfo = damageInfo;
        this.targetUnit = target;
        this.speed = speed;
        this.hasInitialized = true;

        if (target != null)
        {
            lastTargetPos = target.transform.position;
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!hasInitialized)
            return;

        Vector2 currentPos = transform.position;
        Vector2 targetPos = targetUnit != null ? (Vector2)targetUnit.transform.position : lastTargetPos;

        Vector2 direction = targetPos - currentPos;

        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        transform.position = Vector2.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.25f)
        {
            if (targetUnit != null && targetUnit.Health != null)
            {
                OnHit(targetUnit.Health);
            }
            else
            {
                Detonate();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckHit(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckHit(collision.gameObject);
    }

    private void CheckHit(GameObject hitObj)
    {
        if (damageInfo.source != null && hitObj == damageInfo.source)
            return;

        IDamageable damageable = hitObj.GetComponent<IDamageable>();
        if (damageable != null && !damageable.IsDead)
        {
            Unit hitUnit = hitObj.GetComponent<Unit>();
            if (hitUnit != null && damageInfo.source != null)
            {
                Unit srcUnit = damageInfo.source.GetComponent<Unit>();
                if (srcUnit != null && srcUnit.Stats.TeamID == hitUnit.Stats.TeamID)
                {
                    return;
                }
            }

            OnHit(damageable);
        }
    }

    private void OnHit(IDamageable damageable)
    {
        damageable.TakeDamage(damageInfo);
        Detonate();
    }

    private void Detonate()
    {
        if (hitEffect != null)
        {
            GameObject fx = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(fx, 0.6f);
        }
        Destroy(gameObject);
    }
}
