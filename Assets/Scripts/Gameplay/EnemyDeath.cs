using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Düşman canı 0 olduğunda tetiklenir. 
    /// Collider kapatılır, düşman yerçekimiyle aşağı düşer ve 2 saniye sonra yok edilir.
    /// </summary>
    public class EnemyDeath : Simulation.Event<EnemyDeath>
    {
        public EnemyController enemy;

        public override void Execute()
        {
            if (enemy == null) return;

            // Ses efekti çal
            if (enemy._audio && enemy.ouch)
                enemy._audio.PlayOneShot(enemy.ouch);

            // Collider kapat (hasar almayı durdur)
            if (enemy._collider != null)
                enemy._collider.enabled = false;

            // Kontrol kapat (hareket ve saldırıyı durdur)
            if (enemy.control != null)
                enemy.control.enabled = false;

            // Rigidbody2D varsa kinematic'i kapat, yerçekimi devreye girsin → aşağı düşsün
            var rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 3f; // Hızlıca düşsün
            }

            // 2 saniye sonra objeyi tamamen yok et
            Object.Destroy(enemy.gameObject, 2f);
        }
    }
}