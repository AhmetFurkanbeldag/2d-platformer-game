using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class KunaiFixer : Editor
{
    static KunaiFixer()
    {
        EditorApplication.delayCall += FixKunai;
    }

    [MenuItem("Tools/Fix Kunai")]
    public static void FixKunai()
    {
        // 1. Yeni, arka planı %100 transparan ve temiz bir piksel kunai çiziyoruz
        Texture2D tex = new Texture2D(16, 5, TextureFormat.RGBA32, false);
        Color clear = new Color(0,0,0,0);
        Color silver = new Color(0.8f, 0.8f, 0.8f, 1f);
        Color dark = new Color(0.2f, 0.2f, 0.2f, 1f);
        Color handle = new Color(0.4f, 0.1f, 0.1f, 1f); 

        for(int y=0; y<5; y++) {
            for(int x=0; x<16; x++) {
                tex.SetPixel(x, y, clear);
            }
        }
        
        // Handle (Sap)
        for(int x=4; x<=7; x++) {
            tex.SetPixel(x, 2, handle);
        }
        
        // Guard (Siperlik)
        tex.SetPixel(8, 1, dark); tex.SetPixel(8, 2, dark); tex.SetPixel(8, 3, dark);
        
        // Blade (Bıçak kısmı)
        for(int x=9; x<=13; x++) {
            tex.SetPixel(x, 2, silver);
        }
        tex.SetPixel(14, 2, silver); // Uç
        
        // Ring (Halka)
        tex.SetPixel(1, 2, dark); tex.SetPixel(2, 2, dark); tex.SetPixel(3, 2, dark);

        tex.Apply();
        
        byte[] bytes = tex.EncodeToPNG();
        string path = "Assets/Resources/KunaiSprite.png";
        File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if(importer != null) {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 16;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }

        // 2. Kunai Prefab'ı güncelle
        GameObject kunaiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Kunai.prefab");
        if(kunaiPrefab != null) {
            var sr = kunaiPrefab.GetComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            
            // Piksel çizim olduğu için boyutunu düzgün ayarlıyoruz (Daha küçük)
            kunaiPrefab.transform.localScale = new Vector3(0.4f, 0.4f, 1f); 
            
            // Çarpışma kutusunu (BoxCollider2D) yeni resme göre ayarla
            var col = kunaiPrefab.GetComponent<BoxCollider2D>();
            if(col != null) {
                col.size = new Vector2(1f, 0.2f);
            }

            EditorUtility.SetDirty(kunaiPrefab);
            PrefabUtility.SavePrefabAsset(kunaiPrefab);
            Debug.Log(">>> TEMİZ VE TRANSPARAN KUNAI BAŞARIYLA OLUŞTURULDU <<<");
        }
    }
}
