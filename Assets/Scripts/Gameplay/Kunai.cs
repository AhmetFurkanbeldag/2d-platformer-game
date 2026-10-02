using UnityEngine;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Ninja'nın fırlattığı kunai (bıçak).
    /// </summary>
    public class Kunai : MonoBehaviour
    {
        public float speed = 10f;
        public int damage = 15;
        public float direction = 1f;

        void Awake()
        {
            damage = 15; // Inspector'daki ayarı ez ve bıçak hasarını 15 yap
        }

        void Start()
        {
            // 3 saniye sonra sahneden sil (uzaklara giderse)
            Destroy(gameObject, 3f);
        }

        void Update()
        {
            // Yatay eksende hareket et
            transform.position += Vector3.right * direction * speed * Time.deltaTime;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            // Oyuncuya çarptıysa hasar ver
            var player = col.GetComponent<PlayerController>();
            if (player != null)
            {
                var health = player.GetComponent<Health>();
                if (health != null && health.IsAlive)
                {
                    health.Decrement(damage);
                    player.animator.SetTrigger("hurt");
                }
                Destroy(gameObject);
            }
            // Duvara veya yere çarparsa yok ol
            else if (col.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                Destroy(gameObject);
            }
        }
    }
}
