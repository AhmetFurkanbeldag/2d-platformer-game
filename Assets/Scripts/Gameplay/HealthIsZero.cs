using Platformer.Core;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Herhangi bir varlığın canı 0'a düştüğünde tetiklenir.
    /// Oyuncuysa PlayerDeath, düşmansa EnemyDeath olayını başlatır.
    /// </summary>
    public class HealthIsZero : Simulation.Event<HealthIsZero>
    {
        public Health health;

        public override void Execute()
        {
            if (health == null) return;

            // Düşman mı kontrol et
            var enemy = health.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Düşmanın canı bitti — EnemyDeath tetikle
                Schedule<EnemyDeath>().enemy = enemy;
                return;
            }

            // Oyuncunun canı bitti — PlayerDeath tetikle
            Schedule<PlayerDeath>();
        }
    }
}