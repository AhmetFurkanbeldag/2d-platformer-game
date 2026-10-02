using UnityEngine;
using Platformer.Mechanics;

/// <summary>
/// Oyun başladığında tüm düşmanları kontrol eder:
/// - Health bileşeni yoksa ekler
/// - maxHP'yi 100 yapar
/// - Can barını oluşturur
/// - Konumlarını loglar
/// </summary>
public class EnemyDebugger : MonoBehaviour
{
    void Start()
    {
        var enemies = Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        Debug.Log(">>> TOPLAM DUSMAN SAYISI: " + enemies.Length + " <<<");

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            // Health kontrolü
            var h = enemy.GetComponent<Health>();
            if (h == null)
            {
                h = enemy.gameObject.AddComponent<Health>();
                Debug.LogWarning("UYARI: " + enemy.name + " Health bileşeni YOKTU, eklendi!");
            }
            h.maxHP = 100;
            h.ResetHP();

            Debug.Log("Dusman: " + enemy.name 
                + " | Pozisyon: " + enemy.transform.position 
                + " | HP: " + h.CurrentHP + "/" + h.maxHP
                + " | Aktif: " + enemy.gameObject.activeInHierarchy);
        }
    }
}
