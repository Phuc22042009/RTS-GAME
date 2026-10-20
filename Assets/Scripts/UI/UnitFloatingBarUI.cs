using UnityEngine;

public class UnitFloatingBarUI : MonoBehaviour
{
    private Unit owner;
    private Transform barContainer;
    private Transform healthBarFill;
    private Transform manaBarFill;
    private SpriteRenderer healthFillRenderer;
    private SpriteRenderer manaFillRenderer;

    private const float BarWidth = 1.8f;
    private const float HpHeight = 0.22f;
    private const float ManaHeight = 0.14f;
    private const float VerticalOffset = 1.3f;
    private const float HpYOffset = 0.10f;
    private const float ManaYOffset = -0.12f;

    private static Sprite cachedBarSprite;

    private void Awake()
    {
        EnsureBarSprite();
    }

    public void Initialize(Unit owner)
    {
        this.owner = owner;

        CreateBars();

        if (owner.Health != null)
        {
            owner.Health.OnHealthChanged += UpdateHealthBar;
        }

        if (owner.Combat != null)
        {
            owner.Combat.OnManaChanged += UpdateManaBar;
        }

        if (owner.Stats != null)
        {
            UpdateHealthBar(owner.Stats.currentHealth, owner.Stats.maxHealth);
            UpdateManaBar(owner.Stats.currentMana, owner.Stats.maxMana);
        }
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            if (owner.Health != null)
                owner.Health.OnHealthChanged -= UpdateHealthBar;

            if (owner.Combat != null)
                owner.Combat.OnManaChanged -= UpdateManaBar;
        }
    }

    private void LateUpdate()
    {
        if (barContainer != null)
        {
            barContainer.position = transform.position + new Vector3(0f, VerticalOffset, 0f);
            barContainer.rotation = Quaternion.identity;
        }
    }

    private void CreateBars()
    {
        GameObject containerObj = new GameObject("FloatingBars");
        barContainer = containerObj.transform;
        barContainer.position = transform.position + new Vector3(0f, VerticalOffset, 0f);

        GameObject hpBgObj = new GameObject("HpBg");
        hpBgObj.transform.SetParent(barContainer);
        hpBgObj.transform.localPosition = new Vector3(0f, HpYOffset, 0f);
        hpBgObj.transform.localScale = new Vector3(BarWidth + 0.08f, HpHeight + 0.06f, 1f);
        SpriteRenderer hpBgRenderer = hpBgObj.AddComponent<SpriteRenderer>();
        hpBgRenderer.sprite = cachedBarSprite;
        hpBgRenderer.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        hpBgRenderer.sortingOrder = 50;

        GameObject hpFillObj = new GameObject("HpFill");
        hpFillObj.transform.SetParent(barContainer);
        healthBarFill = hpFillObj.transform;
        healthBarFill.localScale = new Vector3(BarWidth, HpHeight, 1f);
        healthBarFill.localPosition = new Vector3(0f, HpYOffset, 0f);
        healthFillRenderer = hpFillObj.AddComponent<SpriteRenderer>();
        healthFillRenderer.sprite = cachedBarSprite;
        Color teamHpColor = owner.Stats.TeamID == 0 ? new Color(0.9f, 0.2f, 0.2f) : new Color(0.2f, 0.8f, 0.3f);
        healthFillRenderer.color = teamHpColor;
        healthFillRenderer.sortingOrder = 51;

        GameObject manaBgObj = new GameObject("ManaBg");
        manaBgObj.transform.SetParent(barContainer);
        manaBgObj.transform.localPosition = new Vector3(0f, ManaYOffset, 0f);
        manaBgObj.transform.localScale = new Vector3(BarWidth + 0.08f, ManaHeight + 0.06f, 1f);
        SpriteRenderer manaBgRenderer = manaBgObj.AddComponent<SpriteRenderer>();
        manaBgRenderer.sprite = cachedBarSprite;
        manaBgRenderer.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        manaBgRenderer.sortingOrder = 50;

        GameObject manaFillObj = new GameObject("ManaFill");
        manaFillObj.transform.SetParent(barContainer);
        manaBarFill = manaFillObj.transform;
        manaBarFill.localScale = new Vector3(0f, ManaHeight, 1f);
        manaBarFill.localPosition = new Vector3(-BarWidth * 0.5f, ManaYOffset, 0f);
        manaFillRenderer = manaFillObj.AddComponent<SpriteRenderer>();
        manaFillRenderer.sprite = cachedBarSprite;
        manaFillRenderer.color = new Color(0f, 0.8f, 1f, 1f);
        manaFillRenderer.sortingOrder = 51;
    }

    public void UpdateHealthBar(float current, float max)
    {
        if (healthBarFill == null || max <= 0f)
            return;

        float ratio = Mathf.Clamp01(current / max);
        healthBarFill.localScale = new Vector3(ratio * BarWidth, HpHeight, 1f);
        healthBarFill.localPosition = new Vector3((-BarWidth * 0.5f) + (ratio * BarWidth * 0.5f), HpYOffset, 0f);
    }

    public void UpdateManaBar(float current, float max)
    {
        if (manaBarFill == null || max <= 0f)
            return;

        float ratio = Mathf.Clamp01(current / max);
        manaBarFill.localScale = new Vector3(ratio * BarWidth, ManaHeight, 1f);
        manaBarFill.localPosition = new Vector3((-BarWidth * 0.5f) + (ratio * BarWidth * 0.5f), ManaYOffset, 0f);

        if (ratio >= 0.99f && manaFillRenderer != null)
        {
            manaFillRenderer.color = new Color(1f, 0.9f, 0.2f, 1f);
        }
        else if (manaFillRenderer != null)
        {
            manaFillRenderer.color = new Color(0f, 0.8f, 1f, 1f);
        }
    }

    public void SetVisible(bool visible)
    {
        if (barContainer != null)
        {
            barContainer.gameObject.SetActive(visible);
        }
    }

    private static void EnsureBarSprite()
    {
        if (cachedBarSprite == null)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.SetPixel(0, 0, Color.white);
            tex.SetPixel(0, 1, Color.white);
            tex.SetPixel(1, 0, Color.white);
            tex.SetPixel(1, 1, Color.white);
            tex.Apply();
            cachedBarSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
        }
    }
}
