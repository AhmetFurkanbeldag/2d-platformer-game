using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class ForceCleanScene {
    [MenuItem("Tools/Force Clean Scene")]
    public static void Run() {
        string[] keywords = { "Tree", "Cloud", "Wall", "Fence", "Prop", "Bush", "Rock", "Grass", "Decoration", "Detail" };
        
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int deletedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            string nameLower = obj.name.ToLower();

            bool shouldDelete = false;
            foreach (string keyword in keywords)
            {
                if (nameLower.Contains(keyword.ToLower()))
                {
                    shouldDelete = true;
                    break;
                }
            }

            Tilemap tm = obj.GetComponent<Tilemap>();
            if (tm != null && (nameLower.Contains("prop") || nameLower.Contains("detail") || nameLower.Contains("cloud")))
            {
                tm.ClearAllTiles();
                shouldDelete = true;
            }

            if (shouldDelete)
            {
                Object.DestroyImmediate(obj);
                deletedCount++;
            }
        }
        
        Debug.Log(">>> FORCE CLEAN SCENE DONE! Deleted: " + deletedCount + " <<<");
    }
}
