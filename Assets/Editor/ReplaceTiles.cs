using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[InitializeOnLoad]
public class ReplaceTiles : Editor
{
    static ReplaceTiles()
    {
        EditorApplication.delayCall += Replace;
    }

    [MenuItem("Tools/Replace Japanese Tiles")]
    public static void Replace()
    {
        string spritePath = "Assets/Tiles/JapanTile.png";
        
        // Texture ayarlarını yapıyoruz
        TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 1024f; // 1024x1024 resim = 1x1 Unity kare boyutu
            importer.filterMode = FilterMode.Bilinear; // Yüksek çözünürlük için yumuşatma
            importer.SaveAndReimport();
        }

        Sprite newSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (newSprite == null)
        {
            Debug.LogError("Yeni Japon mimarisi sprite'i yuklenemedi!");
            return;
        }

        // Değiştirilecek kiremitler
        string[] tilePaths = {
            "Assets/Tiles/TileFloatingLeftEdge.asset",
            "Assets/Tiles/TileFloatingRightEdge.asset",
            "Assets/Tiles/TileFloatingTileMiddle.asset",
            "Assets/Tiles/TileGround.asset",
            "Assets/Tiles/TileGroundDark.asset",
            "Assets/Tiles/TileGroundTop.asset"
        };

        foreach (var path in tilePaths)
        {
            UnityEngine.Tilemaps.Tile tile = AssetDatabase.LoadAssetAtPath<UnityEngine.Tilemaps.Tile>(path);
            if (tile != null)
            {
                tile.sprite = newSprite;
                EditorUtility.SetDirty(tile);
            }
        }
        
        // Sahnedeki Tilemap'leri güncelle
        var tilemaps = Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        foreach (var tm in tilemaps)
        {
            tm.RefreshAllTiles();
        }

        AssetDatabase.SaveAssets();
        Debug.Log(">>> PLATFORMLAR JAPON MIMARISI (CERCEVELI CATI) ILE DEGISTIRILDI <<<");
    }
}
