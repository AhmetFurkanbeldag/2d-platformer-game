using UnityEngine;
using UnityEditor;
using Platformer.UI;

public class SetupEnemyHealthBars : Editor
{
    [MenuItem("Tools/Setup Enemy Health Bars")]
    public static void Run()
    {
        string prefabPath = "Assets/Prefabs/Enemy.prefab";
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (enemyPrefab != null)
        {
            if (enemyPrefab.GetComponent<EnemyHealthBar>() == null)
            {
                enemyPrefab.AddComponent<EnemyHealthBar>();
                EditorUtility.SetDirty(enemyPrefab);
                AssetDatabase.SaveAssets();
                Debug.Log(">>> DÜŞMANLARA CAN BARI EKLENDİ <<<");
            }
            else
            {
                Debug.Log("Can barı zaten ekli.");
            }
        }
        else
        {
            Debug.LogError("Enemy.prefab bulunamadı!");
        }
    }
}
