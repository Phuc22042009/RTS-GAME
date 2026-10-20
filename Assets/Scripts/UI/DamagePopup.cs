using System.Collections;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMesh textMesh;
    private float moveSpeed = 1.8f;
    private Color textColor;

    public static void Create(Vector3 position, float damageAmount, bool isCritical)
    {
        GameObject popupObj = new GameObject("DamagePopup");
        popupObj.transform.position = position + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 1.6f, 0f);

        DamagePopup popup = popupObj.AddComponent<DamagePopup>();
        popup.Setup(damageAmount, isCritical);
    }

    private void Setup(float damageAmount, bool isCritical)
    {
        textMesh = gameObject.AddComponent<TextMesh>();
        textMesh.text = isCritical ? $"CRIT {Mathf.RoundToInt(damageAmount)}" : $"{Mathf.RoundToInt(damageAmount)}";
        textMesh.characterSize = isCritical ? 0.17f : 0.14f;
        textMesh.fontSize = 28;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;

        textColor = isCritical ? new Color(1f, 0.85f, 0.1f, 1f) : new Color(1f, 0.25f, 0.25f, 1f);
        textMesh.color = textColor;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 100;
        }

        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy()
    {
        float timer = 0f;
        float duration = 0.8f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);

            float alpha = Mathf.Clamp01(1f - (timer / duration));
            textColor.a = alpha;
            if (textMesh != null)
            {
                textMesh.color = textColor;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
