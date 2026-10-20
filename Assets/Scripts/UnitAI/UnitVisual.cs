using System.Collections;
using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    private Unit owner;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private UnitFloatingBarUI floatingBar;

    [Header("Visual Effects")]
    [SerializeField] private GameObject deathEffectPrefab;

    public void Initialize(Unit owner)
    {
        this.owner = owner;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        floatingBar = gameObject.AddComponent<UnitFloatingBarUI>();
        floatingBar.Initialize(owner);

        if (owner.Health != null)
        {
            owner.Health.OnTakeDamage += HandleTakeDamage;
            owner.Health.OnDeath += HandleDeath;
        }

        if (owner.Combat != null)
        {
            owner.Combat.OnAttackStarted += HandleAttackStarted;
            owner.Combat.OnAbilityCast += HandleAbilityCast;
        }
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            if (owner.Health != null)
            {
                owner.Health.OnTakeDamage -= HandleTakeDamage;
                owner.Health.OnDeath -= HandleDeath;
            }

            if (owner.Combat != null)
            {
                owner.Combat.OnAttackStarted -= HandleAttackStarted;
                owner.Combat.OnAbilityCast -= HandleAbilityCast;
            }
        }
    }

    private void HandleTakeDamage(DamageInfo damageInfo)
    {
        StartCoroutine(FlashDamageRoutine());
        DamagePopup.Create(transform.position, damageInfo.amount, damageInfo.isCritical);
    }

    private void HandleAttackStarted(Unit target)
    {
        if (target != null)
        {
            StartCoroutine(AttackPunchRoutine(target.transform.position));
        }
    }

    private void HandleAbilityCast(Unit target)
    {
        StartCoroutine(AbilitySurgeRoutine());
    }

    private void HandleDeath(Unit dyingUnit)
    {
        if (floatingBar != null)
        {
            floatingBar.SetVisible(false);
        }

        if (deathEffectPrefab != null)
        {
            GameObject fx = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 0.8f);
        }

        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FlashDamageRoutine()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.08f);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator AttackPunchRoutine(Vector3 targetPos)
    {
        Vector3 initialPos = transform.position;
        Vector3 forwardOffset = (targetPos - initialPos).normalized * 0.25f;

        float elapsed = 0f;
        while (elapsed < 0.05f)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, initialPos + forwardOffset, elapsed / 0.05f);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos + forwardOffset, initialPos, elapsed / 0.1f);
            yield return null;
        }

        transform.position = initialPos;
    }

    private IEnumerator AbilitySurgeRoutine()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 bigScale = originalScale * 1.3f;

        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, bigScale, elapsed / 0.15f);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < 0.15f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(bigScale, originalScale, elapsed / 0.15f);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    private IEnumerator FadeOutRoutine()
    {
        if (spriteRenderer == null)
            yield break;

        float timer = 0f;
        float fadeDuration = 0.5f;
        Color c = spriteRenderer.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Clamp01(1f - (timer / fadeDuration));
            spriteRenderer.color = c;
            yield return null;
        }

        c.a = 0f;
        spriteRenderer.color = c;
    }

    public void SetDeathEffectPrefab(GameObject prefab)
    {
        deathEffectPrefab = prefab;
    }
}
