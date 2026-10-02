using UnityEngine;
using Platformer.Gameplay;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Oyun başladığında can iksirlerini sahnede görünür konumlara yerleştirir.
    /// GameController veya boş bir objeye eklenebilir.
    /// </summary>
    public class PotionSpawner : MonoBehaviour
    {
        public Sprite potionSprite;
        public int healAmount = 10;

        // SpawnPoint (0.39, -0.49) merkez alınarak hesaplanan
        // zemin düzeyinde, kameranın görebileceği pozisyonlar
        private Vector3[] spawnPositions = new Vector3[]
        {
            new Vector3( -3.5f,  0.6f, 0f),
            new Vector3(  4.5f,  0.6f, 0f),
            new Vector3(  9.0f,  0.6f, 0f),
            new Vector3( -8.0f,  0.6f, 0f),
            new Vector3( 13.5f,  0.6f, 0f),
            new Vector3(-12.5f,  0.6f, 0f),
            new Vector3(  0.5f,  3.8f, 0f),  // Orta yüksek platform
        };

        void Start()
        {
            // Sprite yoksa Resources veya başka yoldan yükle
            if (potionSprite == null)
                potionSprite = Resources.Load<Sprite>("HealthPotion");

            foreach (var pos in spawnPositions)
                SpawnPotion(pos);
        }

        void SpawnPotion(Vector3 position)
        {
            GameObject go = new GameObject("HealthPotion");
            go.transform.position   = position;
            go.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite           = potionSprite;
            sr.sortingLayerName = "Default";
            sr.sortingOrder     = 25;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius    = 0.45f;

            var script = go.AddComponent<HealthPotion>();
            script.healAmount = healAmount;
        }
    }
}
