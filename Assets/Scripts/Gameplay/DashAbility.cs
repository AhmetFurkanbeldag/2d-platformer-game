using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Shift tuşuna basılınca ileri doğru hızlı bir Dash (Kaçış/Atılma) yapar.
    /// Dash sırasında kısa süreliğine çarpışma kapalı kalır (düşmanlardan geçer).
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class DashAbility : MonoBehaviour
    {
        [Header("Dash Ayarları")]
        public float dashSpeed     = 18f;   // Dash hızı
        public float dashDuration  = 0.18f; // Kaç saniye dash yapılsın
        public float dashCooldown  = 0.8f;  // Tekrar dash yapabilmek için bekleme

        private PlayerController player;
        private SpriteRenderer    sr;
        private TrailRenderer     trail;   // Iz efekti

        private bool  isDashing   = false;
        private float cooldownTimer = 0f;

        public bool IsDashing => isDashing;

        void Awake()
        {
            player = GetComponent<PlayerController>();
            sr     = GetComponent<SpriteRenderer>();

            // Otomatik olarak Trail Renderer ekle (iz efekti)
            trail = gameObject.GetComponent<TrailRenderer>();
            if (trail == null) trail = gameObject.AddComponent<TrailRenderer>();

            trail.time             = 0.15f;
            trail.startWidth       = 0.4f;
            trail.endWidth         = 0f;
            trail.enabled          = false;
            trail.material         = new Material(Shader.Find("Sprites/Default"));

            // Kırmızı-turuncu dash izi rengi
            trail.startColor = new Color(1f, 0.3f, 0.1f, 0.8f);
            trail.endColor   = new Color(1f, 0.3f, 0.1f, 0f);
        }

        void Update()
        {
            if (cooldownTimer > 0f)
                cooldownTimer -= Time.deltaTime;

            if (isDashing) return;
            if (!player.controlEnabled) return;

            var kb = Keyboard.current;
            bool dashPressed = kb != null && kb.leftShiftKey.wasPressedThisFrame;

            if (dashPressed && cooldownTimer <= 0f)
                StartCoroutine(DoDash());
        }

        IEnumerator DoDash()
        {
            isDashing       = true;
            cooldownTimer   = dashCooldown;
            trail.enabled   = true;

            // Dash yönü: karakterin baktığı yön
            float dir = sr.flipX ? -1f : 1f;

            float elapsed = 0f;
            while (elapsed < dashDuration)
            {
                elapsed += Time.deltaTime;

                // Dash boyunca hızı doğrudan uygula
                player.velocity.x = dir * dashSpeed;
                player.velocity.y = 0f; // Yerçekimini geçici durdur

                // Her karede karakteri hafif transparan yap (hız hissi)
                float alpha = Mathf.Lerp(0.4f, 1f, elapsed / dashDuration);
                sr.color = new Color(1f, 0.5f, 0.2f, alpha); // turuncu parlama

                yield return null;
            }

            // Dash bitti, normale dön
            sr.color    = Color.white;
            trail.enabled = false;
            isDashing   = false;
        }
    }
}
