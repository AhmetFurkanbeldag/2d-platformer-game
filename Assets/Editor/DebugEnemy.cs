using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Platformer.Mechanics;

public class DebugEnemy : MonoBehaviour
{
    [MenuItem("Tools/Debug Enemy Info")]
    public static void DoDebug()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var enemies = GameObject.FindObjectsOfType<EnemyController>();
        foreach(var e in enemies)
        {
            Debug.Log($"Enemy at {e.transform.position} | detectionRange: {e.detectionRange} | attackRange: {e.attackRange} | playerTransform: {(GameObject.FindGameObjectWithTag("Player") != null ? GameObject.FindGameObjectWithTag("Player").transform.position : "NULL")}");
        }
    }
}
