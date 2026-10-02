using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Devriye")]
        public PatrolPath path;
        public AudioClip throwSound;
        public AudioClip ouch;

        // Can Barı iç değişkenler
        private UnityEngine.UI.Image _hpFill;
        private GameObject           _hpBarRoot;

        [Header("Saldırı Ayarları")]
        public float detectionRange  = 10f;   // Fark etme mesafesi
        public float attackRange     = 6f;    // Bıçak fırlatma menzili
        public float meleeRange      = 2.5f;  // Yakın dövüş (yumruk/tekme) menzili
        public float attackCooldown  = 1.5f;  
        
        public int meleeDamage = 10;

        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        Transform playerTransform;
        Health playerHealth;
        float attackTimer = 0f;
        GameObject kunaiPrefab;

        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control        = GetComponent<AnimationController>();
            _collider      = GetComponent<Collider2D>();
            _audio         = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Eğer Unity eski prefab ayarlarından dolayı bu değerleri 0 yaptıysa düzelt
            if (detectionRange <= 0f) detectionRange = 10f;
            if (attackRange <= 0f) attackRange = 6f;
            if (meleeRange <= 0f) meleeRange = 2.5f;
            if (meleeDamage <= 0) meleeDamage = 10; // Tekme/Yumruk hasarı
            attackCooldown = 1f; // Zorla 1 saniye yap

            // Ninjaların canını oyuncuyla aynı (100) yapıyoruz.
            var h = GetComponent<Health>();
            if (h != null)
            {
                h.maxHP = 100;
                h.ResetHP();
            }

            // Kılıç savurma efekti ve sesleri için modüller eklendi
            if (GetComponent<SwordSlashEffect>() == null)
                gameObject.AddComponent<SwordSlashEffect>();
            if (GetComponent<CombatSFX>() == null)
                gameObject.AddComponent<CombatSFX>();

            // Can barını bu nesne içinde yarat
            BuildHealthBar();
        }

        void BuildHealthBar()
        {
            // Eski bar varsa temizle
            var old = transform.Find("EnemyHPBar");
            if (old != null) Destroy(old.gameObject);

            _hpBarRoot = new GameObject("EnemyHPBar");
            _hpBarRoot.transform.SetParent(transform, false);
            _hpBarRoot.transform.localPosition = new Vector3(0f, 1.4f, 0f);
            _hpBarRoot.transform.localScale    = Vector3.one;

            var canvas = _hpBarRoot.AddComponent<Canvas>();
            canvas.renderMode       = RenderMode.WorldSpace;
            canvas.sortingLayerName = "Default";
            canvas.sortingOrder     = 99;

            var crt = _hpBarRoot.GetComponent<RectTransform>();
            crt.sizeDelta = new Vector2(1.2f, 0.18f);

            // Arka plan
            var bg    = new GameObject("BG");
            bg.transform.SetParent(_hpBarRoot.transform, false);
            var bgImg = bg.AddComponent<UnityEngine.UI.Image>();
            bgImg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
            var bgRT  = bg.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;

            // Dolu kısım (kırmızı — başlangıçta %100 tam dolu)
            var fill    = new GameObject("Fill");
            fill.transform.SetParent(_hpBarRoot.transform, false);
            _hpFill      = fill.AddComponent<UnityEngine.UI.Image>();
            _hpFill.color = new Color(0.9f, 0.15f, 0.15f, 1f);

            var fillRT    = fill.GetComponent<RectTransform>();
            fillRT.anchorMin     = Vector2.zero;          // Sol-alt köşe
            fillRT.anchorMax     = Vector2.one;            // Sağ-üst köşe = TAM DOLU
            fillRT.offsetMin     = Vector2.zero;           // Hiç boşluk bırakma
            fillRT.offsetMax     = Vector2.zero;           // Hiç boşluk bırakma
            fillRT.pivot         = new Vector2(0f, 0.5f);  // Sol kenardan küçülsün
        }

        void RefreshBar()
        {
            var h = GetComponent<Health>();
            if (h == null || _hpFill == null) return;
            float ratio = Mathf.Clamp01((float)h.CurrentHP / h.maxHP);
            // Sadece sağ kenarı (anchorMax.x) orana göre ayarlıyoruz
            // ratio=1 → tam dolu kırmızı, ratio=0.5 → yarım, ratio=0 → boş
            _hpFill.rectTransform.anchorMax = new Vector2(ratio, 1f);
            if (_hpBarRoot != null) _hpBarRoot.SetActive(h.IsAlive);
        }

        void Start()
        {
            var playerObj = Object.FindFirstObjectByType<PlayerController>();
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
                playerHealth    = playerObj.GetComponent<Health>();
            }
            kunaiPrefab = Resources.Load<GameObject>("Kunai");
        }

        void Update()
        {
            RefreshBar(); // Can barını her karede güncelle

            if (attackTimer > 0f)
                attackTimer -= Time.deltaTime;

            if (playerTransform == null)
            {
                var playerObj = Object.FindFirstObjectByType<PlayerController>();
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                    playerHealth    = playerObj.GetComponent<Health>();
                }
                return;
            }

            float distX = playerTransform.position.x - transform.position.x;
            float dist  = Vector2.Distance(transform.position, playerTransform.position);

            // Düşmanın baktığı yön (Sağ = 1, Sol = -1)
            float facingDir = spriteRenderer.flipX ? -1f : 1f;

            // Oyuncu düşmanın görüş açısında mı? (Baktığı yönde mi?)
            bool inFieldOfView = (Mathf.Sign(distX) == facingDir);

            // Çok yakınındaysa arkasında bile olsa fark etsin (Ayak sesi duydu gibi)
            bool isCloseEnoughToHear = (dist <= 2.5f);

            if (dist <= detectionRange && (inFieldOfView || isCloseEnoughToHear))
            {
                spriteRenderer.flipX = (distX < 0f);

                if (dist <= attackRange)
                {
                    // Saldırı menzilindeyse ve saldıracaksa dur
                    control.move.x = 0f;

                    if (attackTimer <= 0f && playerHealth != null && playerHealth.IsAlive)
                    {
                        // Oyuncu şu an kılıç sallıyor veya yumruk atıyorsa düşman bloklanır/bekler
                        var playerCtrl = playerHealth.GetComponent<PlayerController>();
                        bool isPlayerAttackingUs = false;
                        if (playerCtrl != null && playerCtrl.isAttacking)
                        {
                            float pDistX = transform.position.x - playerTransform.position.x;
                            float pFacing = playerCtrl.GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
                            if (Mathf.Sign(pDistX) == pFacing)
                                isPlayerAttackingUs = true;
                        }

                        if (!isPlayerAttackingUs)
                        {
                            attackTimer = attackCooldown;
                            
                            // Çok yakınsa dövüş, yoksa bıçak fırlat
                            if (dist <= meleeRange)
                            {
                                MeleeAttack(Mathf.Sign(distX));
                            }
                            else
                            {
                                ThrowKunai(Mathf.Sign(distX));
                            }
                        }
                    }
                }
            }
            else
            {
                // Devriye
                if (path != null)
                {
                    if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                    control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1f, 1f);
                }
                else
                {
                    control.move.x = 0f;
                }
            }
        }

        void MeleeAttack(float dir)
        {
            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                // Her zaman kılıç sallama animasyonu kullan
                animator.SetTrigger("attack");
            }

            // Kılıç İzi Efekti
            var slash = GetComponent<SwordSlashEffect>();
            if (slash != null)
            {
                slash.PlaySlash(); // Yönü kendisi SpriteRenderer'dan alıyor
            }

            // Kılıç Sesi
            var sfx = GetComponent<CombatSFX>();
            if (sfx != null)
            {
                sfx.PlaySwordSwing();
            }

            if (playerHealth != null && playerHealth.IsAlive)
            {
                playerHealth.Decrement(meleeDamage);
                var pController = playerHealth.GetComponent<PlayerController>();
                if (pController != null && pController.animator != null)
                {
                    pController.animator.SetTrigger("hurt");
                    pController.Bounce(4f); // Oyuncu hasar alınca hafifçe sarsılıp geriye seker
                    pController.FlashHurt(); // Oyuncuya özel hasar efekti
                }
            }
        }

        void ThrowKunai(float dir)
        {
            if (kunaiPrefab != null)
            {
                Vector3 spawnPos = transform.position + new Vector3(dir * 0.8f, 0.2f, 0f);
                var go = Instantiate(kunaiPrefab, spawnPos, Quaternion.identity);
                
                var kunai = go.GetComponent<Kunai>();
                if (kunai != null)
                {
                    kunai.direction = dir;
                    go.GetComponent<SpriteRenderer>().flipX = (dir < 0);
                }

                if (_audio != null && throwSound != null)
                    _audio.PlayOneShot(throwSound, 0.7f);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev  = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy  = this;
            }
        } // <- EKSİK OLAN PARANTEZ BURASI

        public void BounceEnemy(float force = 5f)
        {
            // Sadece yatayda hafifçe geriye it, havaya zıplatma
            if (control != null)
            {
                float bounceDir = spriteRenderer.flipX ? 1f : -1f;
                control.velocity = new Vector2(bounceDir * force, 0f);
                
                if (_audio != null && throwSound != null)
                    _audio.PlayOneShot(throwSound, 0.5f);
            }
        }

        public void FlashHurt()
        {
            if (spriteRenderer != null && gameObject.activeInHierarchy)
                StartCoroutine(HurtFlashRoutine());
        }

        System.Collections.IEnumerator HurtFlashRoutine()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
}