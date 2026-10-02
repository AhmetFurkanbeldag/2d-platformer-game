using UnityEngine;
using UnityEditor;

public class FindPlayers : MonoBehaviour
{
    [MenuItem("Tools/Find Players")]
    public static void DoFind()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        var players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"FOUND {players.Length} OBJECTS WITH PLAYER TAG:");
        foreach(var p in players)
        {
            Debug.Log($" - {p.name} at {p.transform.position}");
        }
    }
}
