using UnityEngine;
using UnityEditor;
using Platformer.Gameplay;

public class PlacePotions : Editor
{
    [MenuItem("Tools/Place Health Potions")]
    public static void Place()
    {
        // 1. Sprite import ayarlarını yap
        string spritePath = "Assets/Tiles/HealthPotion.png";
        TextureImporter imp = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        if (imp != null)
        {
            imp.textureType         = TextureImporterType.Sprite;
            imp.spriteImportMode    = SpriteImportMode.Single;
            imp.spritePixelsPerUnit = 512f;
            imp.filterMode          = FilterMode.Point; // Pixel art için keskin kenar
            imp.alphaIsTransparency = true;
            imp.SaveAndReimport();
        }

        Sprite potionSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (potionSprite == null)
        {
            Debug.LogError("HealthPotion sprite yuklenemedi: " + spritePath);
            return;
        }

        // 2. Eski potionları temizle (tekrar çalıştırılırsa çift koymasın)
        GameObject[] existing = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var go in existing)
            if (go.name.Contains("HealthPotion_")) Object.DestroyImmediate(go);

        // 3. Haritanın gizli köşelerine potionları yerleştir
        // Koordinatlar haritaya göre ayarlandı — platformların kenarlarında/gizli noktalarda
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-12f,  2f, 0f),   // Sol üst gizli köşe
            new Vector3( 14f,  2f, 0f),   // Sağ üst gizli köşe
            new Vector3(  0f, -1f, 0f),   // Orta zemin yakını
            new Vector3( -6f,  5f, 0f),   // Sol orta platform üstü
            new Vector3(  8f,  5f, 0f),   // Sağ orta platform üstü
            new Vector3(-16f, -1f, 0f),   // Sol uç gizli
            new Vector3( 18f, -1f, 0f),   // Sağ uç gizli
        };

        GameObject container = new GameObject("Potions");

        foreach (var pos in positions)
        {
            // Potion objesini oluştur
            GameObject potionGO = new GameObject("HealthPotion_" + pos.x + "_" + pos.y);
            potionGO.transform.SetParent(container.transform);
            potionGO.transform.position = pos;
            potionGO.transform.localScale = new Vector3(0.5f, 0.5f, 1f); // Küçük boyut

            // Sprite Renderer ekle
            var sr = potionGO.AddComponent<SpriteRenderer>();
            sr.sprite = potionSprite;
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 10;

            // Trigger Collider ekle (sadece tetikleyici, fizik duvarı değil)
            var col = potionGO.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;

            // HealthPotion script ekle
            var script = potionGO.AddComponent<HealthPotion>();
            script.healAmount = 10;
        }

        Debug.Log(">>> " + positions.Length + " ADET CAN IKSIRI HARITAYA YERLESTIRILDI <<<");
    }
}
