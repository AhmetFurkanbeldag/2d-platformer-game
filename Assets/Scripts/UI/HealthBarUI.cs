using UnityEngine;
using UnityEngine.UI;
using Platformer.Mechanics;

namespace Platformer.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        public Health playerHealth;
        public Image screenFlash; 

        RectTransform barRect;
        Image bgImage;
        Image fillImage;

        int lastHP = -1;
        float flashTimer = 0f;
        const float FlashDuration = 0.2f;

        void Start()
        {
            if (playerHealth == null)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    playerHealth = playerObj.GetComponent<Health>();
            }

            SetupProgressBar();
        }

        void SetupProgressBar()
        {
            // Eski kalpleri saklayan paneli bulup kapat (eğer varsa)
            var panel = transform.Find("HeartPanel");
            if (panel != null) panel.gameObject.SetActive(false);

            // Yeni progress bar oluştur
            GameObject barObj = new GameObject("HealthProgressBar");
            barObj.transform.SetParent(transform, false);
            barRect = barObj.AddComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0, 1);
            barRect.anchorMax = new Vector2(0, 1);
            barRect.pivot = new Vector2(0, 1);
            barRect.anchoredPosition = new Vector2(20, -20);
            barRect.sizeDelta = new Vector2(300, 25);

            // Arka planı siyah/gri yap
            GameObject bgObj = new GameObject("BG");
            bgObj.transform.SetParent(barRect, false);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

            // Kırmızı dolgu (Fill)
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(barRect, false);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero; fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.9f, 0.15f, 0.15f, 1f);
            
            // Doldurulabilir sprite mantığını kurmak için boş bir beyaz sprite yarat
            fillImage.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,4,4), Vector2.zero);
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        }

        void Update()
        {
            if (playerHealth == null) return;

            int currentHP = playerHealth.CurrentHP;

            if (currentHP != lastHP)
            {
                // Can azaldıysa flash animasyonu
                if (lastHP != -1 && currentHP < lastHP && screenFlash != null)
                {
                    screenFlash.color = new Color(1f, 0f, 0f, 0.35f);
                    flashTimer = FlashDuration;
                }
                lastHP = currentHP;

                // Barın uzunluğunu ayarla
                if (fillImage != null && playerHealth.maxHP > 0)
                {
                    fillImage.fillAmount = (float)currentHP / playerHealth.maxHP;
                }
            }

            if (flashTimer > 0f)
            {
                flashTimer -= Time.deltaTime;
                if (screenFlash != null)
                {
                    float a = Mathf.Clamp01(flashTimer / FlashDuration) * 0.35f;
                    screenFlash.color = new Color(1f, 0f, 0f, a);
                }
            }
        }
    }
}
