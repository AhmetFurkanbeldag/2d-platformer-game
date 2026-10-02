using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[InitializeOnLoad]
public class CleanScene : Editor
{
    static CleanScene()
    {
        EditorApplication.delayCall += CleanUp;
    }

    [MenuItem("Tools/Clean Up Scene")]
    public static void CleanUp()
    {
        string[] keywords = { "Tree", "Cloud", "Wall", "Fence", "Prop", "Bush", "Rock", "Grass", "Decoration", "Detail", "BackgroundTilemap" };
        
        // Sahnede bulunan tüm objeleri al
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int deletedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;

            string nameLower = obj.name.ToLower();

            // İsim eşleştirme ile ağaç, bulut, çit vs. sil
            bool shouldDelete = false;
            foreach (string keyword in keywords)
            {
                if (nameLower.Contains(keyword.ToLower()))
                {
                    shouldDelete = true;
                    break;
                }
            }

            // Tilemap'ler için özel kontrol
            Tilemap tm = obj.GetComponent<Tilemap>();
            if (tm != null)
            {
                // Ana zemin dışındaki dekorasyon tilemap'lerini temizle (sadece zemin/platform kalmalı)
                // Orijinal Microgame'de zemin genelde "Tilemap" veya "Ground" olarak adlandırılır. 
                // Eğer "Background", "Props" veya dekorasyon kelimeleri içeriyorsa o tilemap'in içini sil
                if (nameLower.Contains("prop") || nameLower.Contains("detail") || nameLower.Contains("back") || nameLower.Contains("decor") || nameLower.Contains("cloud"))
                {
                    tm.ClearAllTiles();
                    shouldDelete = true;
                }
            }

            if (shouldDelete && obj.transform.parent != null)
            {
                // Root obje değilse sadece kendini yok et
                Undo.DestroyObjectImmediate(obj);
                deletedCount++;
            }
            else if (shouldDelete && obj.transform.parent == null)
            {
                // Sahnede kök obje ise (Prefab kökü vs.)
                Undo.DestroyObjectImmediate(obj);
                deletedCount++;
            }
        }
        
        // Belki arka planın kendi kamerası veya bir resim objesi var, 
        // ana "Background" veya "Canvas" olanlara dokunmuyoruz ki eklediğimiz yeni ay manzarası silinmesin.

        Debug.Log(">>> SAHNE TEMIZLENDI! Toplam silinen dekorasyon objesi: " + deletedCount + " <<<");
    }
}
