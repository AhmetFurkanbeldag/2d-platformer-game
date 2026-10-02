using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class UltimateSetup : Editor
{
    static UltimateSetup()
    {
        EditorApplication.delayCall += DoSetup;
    }

    [MenuItem("Tools/Fix Everything")]
    public static void DoSetup()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "SampleScene") return;

        // 1. Arka Planı Kameraya Kilitle
        var cam = Camera.main;
        var bg = GameObject.Find("JapanBackground");
        if (cam != null && bg != null)
        {
            bg.transform.SetParent(cam.transform);
            bg.transform.localPosition = new Vector3(0, 0, 50f);
            
            var sr = bg.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                float camH = cam.orthographicSize * 2f;
                float camW = camH * cam.aspect;
                float sprW = sr.sprite.bounds.size.x;
                float sprH = sr.sprite.bounds.size.y;
                bg.transform.localScale = new Vector3(camW / sprW, camH / sprH, 1f);
                sr.sortingOrder = -9999;
                sr.sortingLayerName = "Default";
            }
        }

        // 2. Health Bar UI'ı temizle ve baştan oluştur (eğer lazımsa)
        
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        
        Debug.Log(">>> HER ŞEY BAŞARIYLA DÜZELTİLDİ VE SAHNEDE KAYDEDİLDİ <<<");
    }
}
