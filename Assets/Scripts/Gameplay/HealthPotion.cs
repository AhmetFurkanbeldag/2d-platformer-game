using System.Collections;
using UnityEngine;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Can iksiri: Karaktere çarpınca 10 HP verir, sonra kendini yok eder.
    /// Görsel olarak yukarı-aşağı sallanır (bob animasyonu) ve hafif parlar.
    /// </summary>
    public class HealthPotion : MonoBehaviour
    {
        [Header("Potion Settings")]
        public int healAmount = 10;

        private SpriteRenderer sr;
        private float bobTimer = 0f;
        private Vector3 startPos;

        // Parlama için renk döngüsü
        private float glowTimer = 0f;

        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            startPos = transform.position;
        }

        void Update()
        {
            // Bob animasyonu (yukarı-aşağı sallanma)
            bobTimer += Time.deltaTime * 2f;
            transform.position = startPos + new Vector3(0f, Mathf.Sin(bobTimer) * 0.12f, 0f);

            // Parlama efekti (kırmızıdan beyaza dönen glow)
            glowTimer += Time.deltaTime * 3f;
            float glow = (Mathf.Sin(glowTimer) + 1f) / 2f; // 0-1 arası
            sr.color = Color.Lerp(new Color(1f, 0.4f, 0.4f, 1f), Color.white, glow * 0.4f);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null) return;

            var health = player.GetComponent<Health>();
            if (health == null) return;

            // Sadece tam dolu değilse al (yer boşa gitmesin)
            if (health.CurrentHP >= health.maxHP) return;

            // 10 HP iyileştir (tek seferde)
            health.IncrementBy(healAmount);

            // Küçük partikel efekti
            StartCoroutine(PickupEffect());
        }

        IEnumerator PickupEffect()
        {
            // Hızla büyüyüp kaybol
            float t = 0f;
            Vector3 original = transform.localScale;
            while (t < 0.25f)
            {
                t += Time.deltaTime;
                float s = Mathf.Lerp(1f, 2.5f, t / 0.25f);
                transform.localScale = original * s;
                sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, t / 0.25f));
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
