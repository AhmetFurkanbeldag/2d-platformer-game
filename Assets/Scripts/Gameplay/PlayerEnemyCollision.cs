using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;

            if (willHurtEnemy)
            {
                var enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null && enemyHealth.IsAlive)
                {
                    // Kafaya zıplama = 15 hasar (öldürmez, sadece canı azaltır)
                    enemyHealth.Decrement(15);
                    
                    // Ninja'yı geriye savur ve kırmızı parlat
                    enemy.BounceEnemy(4f);
                    enemy.FlashHurt();

                    if (!enemyHealth.IsAlive)
                    {
                        Schedule<EnemyDeath>().enemy = enemy;
                    }

                    // Oyuncuyu yukarı zıplat
                    player.Bounce(7);
                }
            }
            else
            {
                // Düşmanın kafasına zıplamadıysak sadece hasar alalım ve hafif geriye sekelim
                var playerHealth = player.GetComponent<Health>();
                if (playerHealth != null && playerHealth.IsAlive)
                {
                    playerHealth.Decrement(10);
                    player.Bounce(4);
                    if (player.animator != null) player.animator.SetTrigger("hurt");
                    player.FlashHurt();
                }
            }
        }
    }
}