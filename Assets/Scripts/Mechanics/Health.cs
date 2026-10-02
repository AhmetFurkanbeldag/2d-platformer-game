using System;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Represebts the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviour
    {
        /// <summary>
        /// The maximum hit points for the entity.
        /// </summary>
        public int maxHP = 50;

        /// <summary>
        /// Indicates if the entity should be considered 'alive'.
        /// </summary>
        public bool IsAlive => currentHP > 0;

        /// <summary>
        /// Current HP — HealthBarUI tarafından okunur.
        /// </summary>
        public int CurrentHP => currentHP;

        int currentHP;

        /// <summary>
        /// Increment the HP of the entity.
        /// </summary>
        public void Increment()
        {
            currentHP = Mathf.Clamp(currentHP + 1, 0, maxHP);
        }

        /// <summary>
        /// Belirtilen miktar kadar HP ekle (Can iksiri vb. için)
        /// </summary>
        public void IncrementBy(int amount)
        {
            currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        }

        /// <summary>
        /// Decrement the HP of the entity. Will trigger a HealthIsZero event when
        /// current HP reaches 0.
        /// </summary>
        public void Decrement(int amount = 10)
        {
            currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
            if (currentHP == 0)
            {
                var ev = Schedule<HealthIsZero>();
                ev.health = this;
            }
        }

        /// <summary>
        /// Decrement the HP of the entitiy until HP reaches 0.
        /// </summary>
        public void Die()
        {
            while (currentHP > 0) Decrement();
        }

        public void ResetHP()
        {
            currentHP = maxHP;
        }

        void Awake()
        {
            // Oyuncuysa canı kesinlikle 200 yap (Eski prefab ayarlarını ezmek için ve daha dayanıklı olmak için)
            if (gameObject.CompareTag("Player"))
            {
                maxHP = 200;
            }
            // Düşmansa onun da canını oyuncuyla aynı (100) yap
            else if (gameObject.GetComponent<EnemyController>() != null)
            {
                maxHP = 100;
            }
            currentHP = maxHP;
        }
    }
}
