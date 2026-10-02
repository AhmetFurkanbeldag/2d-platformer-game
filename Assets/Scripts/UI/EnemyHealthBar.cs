using UnityEngine;
using UnityEngine.UI;
using Platformer.Mechanics;

// Namespace kullanmıyoruz — EnemyController'dan direkt erişim için global scope'ta tutuyoruz
/// <summary>
/// Düşmanın tepesinde can çubuğu gösterir. EnemyController tarafından Awake'te otomatik eklenir.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    private Health        health;
    private RectTransform fillRT;
    private GameObject    barRoot;

    public void Initialize()
    {
        health = GetComponent<Health>();
        if (health == null) return;

        // --- Eski bar varsa sil ---
        var old = transform.Find("EnemyHPBar");
        if (old != null) Destroy(old.gameObject);

        // --- Ana canvas ---
        barRoot = new GameObject("EnemyHPBar");
        barRoot.transform.SetParent(transform, false);
        barRoot.transform.localPosition = new Vector3(0f, 1.4f, 0f);
        barRoot.transform.localScale    = Vector3.one;

        var canvas = barRoot.AddComponent<Canvas>();
        canvas.renderMode       = RenderMode.WorldSpace;
        canvas.sortingLayerName = "Default";
        canvas.sortingOrder     = 99;

        var crt = barRoot.GetComponent<RectTransform>();
        crt.sizeDelta = new Vector2(1.2f, 0.18f);

        // --- Arka plan ---
        var bg    = new GameObject("BG");
        bg.transform.SetParent(barRoot.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        var bgRT  = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // --- Dolu kısım (kırmızı) ---
        var fill    = new GameObject("Fill");
        fill.transform.SetParent(barRoot.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.9f, 0.15f, 0.15f, 1f);

        fillRT           = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0f, 0.05f);
        fillRT.anchorMax = new Vector2(1f, 0.95f);
        fillRT.offsetMin = new Vector2(2f,  0f);
        fillRT.offsetMax = new Vector2(-2f, 0f);
        fillRT.pivot     = new Vector2(0f, 0.5f);

        Refresh();
    }

    void Update()
    {
        if (health == null || fillRT == null) return;
        Refresh();
        if (barRoot != null)
            barRoot.SetActive(health.IsAlive);
    }

    void Refresh()
    {
        if (health == null || fillRT == null) return;
        float ratio = Mathf.Clamp01((float)health.CurrentHP / health.maxHP);
        fillRT.anchorMax = new Vector2(ratio, fillRT.anchorMax.y);
    }
}
