using UnityEngine;
using UnityEditor;
using Platformer.Gameplay;

public class SetupDashAndPotions : Editor
{
    [MenuItem("Tools/Setup Dash + Potions")]
    public static void Run()
    {
        // ==========================================
        // 1. DASH — Player prefab'ına ekle
        // ==========================================
        string playerPath = "Assets/Prefabs/Player.prefab";
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerPath);
        if (playerPrefab != null)
        {
            // Dash zaten ekliyse tekrar ekleme
            if (playerPrefab.GetComponent<DashAbility>() == null)
            {
                var dash = playerPrefab.AddComponent<DashAbility>();
                dash.dashSpeed    = 18f;
                dash.dashDuration = 0.18f;
                dash.dashCooldown = 0.8f;
                EditorUtility.SetDirty(playerPrefab);
                Debug.Log(">>> DASH EKLENDI: Player.prefab <<<");
            }
            else
            {
                Debug.Log("Dash zaten Player.prefab'da mevcut.");
            }
        }
        else
        {
            Debug.LogError("Player.prefab bulunamadi!");
        }

        // ==========================================
        // 2. POTIONS — Görünür, başlangıç bölgesi etrafına yerleştir
        // SpawnPoint (0.39, -0.49) merkez alındı.
        // Tüm potionlar zemin seviyesinde veya hemen üzerinde.
        // ==========================================
        string spritePath = "Assets/Tiles/HealthPotion.png";
        TextureImporter imp = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        if (imp != null)
        {
            imp.textureType         = TextureImporterType.Sprite;
            imp.spriteImportMode    = SpriteImportMode.Single;
            imp.spritePixelsPerUnit = 256f;
            imp.filterMode          = FilterMode.Point;
            imp.alphaIsTransparency = true;
            imp.SaveAndReimport();
        }

        Sprite potionSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (potionSprite == null)
        {
            Debug.LogError("HealthPotion sprite yuklenemedi!");
            return;
        }

        // Eski potionları temizle
        var oldContainer = GameObject.Find("Potions");
        if (oldContainer != null) Object.DestroyImmediate(oldContainer);

        // Potion konumları: SpawnPoint etrafında, zemin üstünde, açıkça görünür yerlerde
        // (Tüm X değerleri kameranın gördüğü oyun alanı içinde)
        Vector3[] positions = new Vector3[]
        {
            new Vector3( -3f,  0.5f, 0f),  // SpawnPoint solu, zemin üstü
            new Vector3(  4f,  0.5f, 0f),  // SpawnPoint sağı, zemin üstü
            new Vector3(  9f,  0.5f, 0f),  // Sağda görünür
            new Vector3(-8f,   0.5f, 0f),  // Solda görünür
            new Vector3( 14f,  0.5f, 0f),  // Daha sağda
            new Vector3(-13f,  0.5f, 0f),  // Daha solda
            new Vector3(  0f,  3.5f, 0f),  // Orta, yüksek platform
        };

        GameObject container = new GameObject("Potions");

        int idx = 0;
        foreach (var pos in positions)
        {
            GameObject go = new GameObject("HealthPotion_" + idx);
            go.transform.SetParent(container.transform);
            go.transform.position   = pos;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite           = potionSprite;
            sr.sortingLayerName = "Default";
            sr.sortingOrder     = 20; // Her şeyin üstünde görünsün

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius    = 0.4f;

            var script = go.AddComponent<HealthPotion>();
            script.healAmount = 10;

            idx++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log(">>> DASH + " + positions.Length + " POTION BASARIYLA EKLENDI <<<");
    }
}
