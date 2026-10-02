using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.InputSystem;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        private InputAction m_MoveAction;
        private InputAction m_JumpAction;
        private InputAction m_AttackAction;

        // Saldırı cooldown
        private float attackCooldown = 0f;
        private const float AttackCooldownDuration = 0.5f; // Saldırı hızını artırdım
        public bool isAttacking = false;

        // -------- MOBİL KONTROLLER --------
        [HideInInspector] public float mobileMoveX = 0f;
        [HideInInspector] public bool mobileJumpPressed = false;
        [HideInInspector] public bool mobileJumpReleased = false;
        [HideInInspector] public bool mobileSwordPressed = false;
        // ----------------------------------

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            // Dash yoksa otomatik ekle
            if (GetComponent<DashAbility>() == null)
                gameObject.AddComponent<DashAbility>();

            // Savaş ses efektleri
            if (GetComponent<CombatSFX>() == null)
                gameObject.AddComponent<CombatSFX>();

            // Kılıç izi (slash) efekti
            if (GetComponent<SwordSlashEffect>() == null)
                gameObject.AddComponent<SwordSlashEffect>();

            m_MoveAction = InputSystem.actions.FindAction("Player/Move");
            m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
            m_AttackAction = InputSystem.actions.FindAction("Player/Attack");

            m_MoveAction.Enable();
            m_JumpAction.Enable();
            if (m_AttackAction != null) m_AttackAction.Enable();
        }

        protected override void Update()
        {
            // Saldırı cooldown say
            if (attackCooldown > 0f)
            {
                attackCooldown -= Time.deltaTime;
                if (attackCooldown <= 0f)
                    isAttacking = false;
            }

            if (controlEnabled)
            {
                // Bilgisayardan (Klavye/Joystick) gelen yön
                float kbMoveX = m_MoveAction.ReadValue<Vector2>().x;
                // Mobil Joystick'ten gelen yön (eğer klavye kullanılmıyorsa mobil joysticki kullan)
                move.x = Mathf.Abs(kbMoveX) > 0.1f ? kbMoveX : mobileMoveX;

                bool jumpPressed = m_JumpAction.WasPressedThisFrame() || mobileJumpPressed;
                bool jumpReleased = m_JumpAction.WasReleasedThisFrame() || mobileJumpReleased;

                if (jumpState == JumpState.Grounded && jumpPressed)
                    jumpState = JumpState.PrepareToJump;
                else if (jumpReleased)
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                // Tüketilen mobil zıplama bayraklarını sıfırla
                mobileJumpPressed = false;
                mobileJumpReleased = false;

                bool swordAttackPressed = false;
                bool punchAttackPressed = false;

                // Sol Tık (Kılıç) veya Mobil Kılıç Butonu
                var mouse = Mouse.current;
                if ((mouse != null && mouse.leftButton.wasPressedThisFrame) || mobileSwordPressed)
                    swordAttackPressed = true;
                    
                // Tüketilen mobil saldırı bayrağını sıfırla
                mobileSwordPressed = false;

                // J Tuşu (Tekme/Tokat)
                var kb = Keyboard.current;
                if (kb != null && kb.jKey.wasPressedThisFrame)
                    punchAttackPressed = true;

                if ((swordAttackPressed || punchAttackPressed) && attackCooldown <= 0f)
                {
                    isAttacking = true;
                    attackCooldown = AttackCooldownDuration;
                    
                    if (swordAttackPressed)
                    {
                        animator.SetTrigger("attack");
                        var sfx = GetComponent<CombatSFX>();
                        if (sfx != null) sfx.PlaySwordSwing();

                        var slash = GetComponent<SwordSlashEffect>();
                        if (slash != null) slash.PlaySlash();
                    }
                    else
                    {
                        animator.SetTrigger("punch");
                        var sfx = GetComponent<CombatSFX>();
                        if (sfx != null) sfx.PlayKick();
                    }

                    // Hasar Hesaplama
                    int damage = swordAttackPressed ? 20 : 10;
                    
                    var dash = GetComponent<Platformer.Gameplay.DashAbility>();
                    if (dash != null && dash.IsDashing)
                    {
                        damage = 50; // Shiftli (Dash) vuruş bonusu
                    }

                    // Hasar Ver
                    PerformAttackHitbox(damage);
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void PerformAttackHitbox(int damage)
        {
            Vector2 origin = transform.position;
            bool hitSomething = false;

            var enemies = Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
            Debug.Log("Saldiri! Toplam dusman: " + enemies.Length + " | Oyuncu Konum: " + origin);
            
            foreach (var enemy in enemies)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

                float dist = Vector2.Distance(origin, enemy.transform.position);

                // 4 birim içindeki tüm düşmanlara hasar ver
                if (dist <= 4f)
                {
                    var enemyHealth = enemy.GetComponent<Health>();
                    if (enemyHealth != null && enemyHealth.IsAlive)
                    {
                        enemyHealth.Decrement(damage);
                        enemy.FlashHurt();        // Kırmızı parlama
                        enemy.BounceEnemy(3f);    // Geriye savurma efekti
                        hitSomething = true;

                        // İsabet sesi
                        var sfx = GetComponent<CombatSFX>();
                        if (sfx != null) sfx.PlayHitImpact();

                        Debug.Log(">>> VURUS! " + enemy.name + " | Hasar: " + damage + " | Kalan HP: " + enemyHealth.CurrentHP + "/" + enemyHealth.maxHP);
                        
                        if (!enemyHealth.IsAlive)
                        {
                            Schedule<EnemyDeath>().enemy = enemy;
                            if (sfx != null) sfx.PlayEnemyDeath();
                            Debug.Log(">>> " + enemy.name + " OLDU!");
                        }
                    }
                }
                else
                {
                    Debug.Log("Dusman " + enemy.name + " cok uzak: " + dist.ToString("F1") + " birim");
                }
            }

            if (!hitSomething)
                Debug.Log("Menzilde dusman yok!");
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public void FlashHurt()
        {
            if (spriteRenderer != null && gameObject.activeInHierarchy)
                StartCoroutine(HurtFlashRoutine());
        }

        System.Collections.IEnumerator HurtFlashRoutine()
        {
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f, 1f); // Oyuncu açık kırmızı parlasın
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}