using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class KunaiSetup : Editor
{
    static KunaiSetup()
    {
        EditorApplication.delayCall += SetupKunaiPrefab;
    }

    static void SetupKunaiPrefab()
    {
        string resourcesPath = "Assets/Resources";
        string spritePath = "Assets/Character/Sprites/Kunai.png";
        string prefabPath = "Assets/Resources/Kunai.prefab";

        if (!AssetDatabase.IsValidFolder(resourcesPath))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        // Eğer prefab zaten varsa geç
        if (File.Exists(prefabPath)) return;

        // Kunai objesini oluştur
        GameObject kunaiObj = new GameObject("Kunai");
        
        // SpriteRenderer ekle
        var sr = kunaiObj.AddComponent<SpriteRenderer>();
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite != null)
        {
            sr.sprite = sprite;
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 5;
        }

        // Kunai scriptini ekle
        var kunaiScript = kunaiObj.AddComponent<Platformer.Gameplay.Kunai>();
        kunaiScript.speed = 12f;
        kunaiScript.damage = 10;

        // Collider ekle
        var box = kunaiObj.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new Vector2(0.5f, 0.1f);

        // Prefab olarak kaydet
        PrefabUtility.SaveAsPrefabAsset(kunaiObj, prefabPath);
        
        // Sahnedeki temp objeyi sil
        GameObject.DestroyImmediate(kunaiObj);

        Debug.Log("[KunaiSetup] Kunai prefabı başarıyla oluşturuldu: " + prefabPath);
    }
}
