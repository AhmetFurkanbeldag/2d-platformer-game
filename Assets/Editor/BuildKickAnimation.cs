using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[InitializeOnLoad]
public class BuildKickAnimation : Editor
{
    static BuildKickAnimation()
    {
        EditorApplication.delayCall += Build;
    }

    [MenuItem("Tools/Build Kick")]
    public static void Build()
    {
        // Oyuncunun koşma ve zıplama resimlerini birleştirerek sahte bir "Tekme" (Kick) animasyonu oluşturuyoruz
        Sprite s1 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Character/Sprites/Samurai/run_3.png");
        Sprite s2 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Character/Sprites/Samurai/jump_2.png");
        Sprite s3 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Character/Sprites/Samurai/run_5.png");

        if (s1 == null || s2 == null || s3 == null) 
        {
            Debug.LogWarning("Sprite dosyaları bulunamadı! Lütfen yolu kontrol et.");
            return;
        }

        AnimationClip clip = new AnimationClip();
        clip.frameRate = 12;

        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[3];
        keyframes[0] = new ObjectReferenceKeyframe { time = 0f, value = s1 };
        keyframes[1] = new ObjectReferenceKeyframe { time = 0.1f, value = s2 };
        keyframes[2] = new ObjectReferenceKeyframe { time = 0.25f, value = s3 };

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);

        string path = "Assets/Character/Animations/SamuraiKick.anim";
        
        // Varsa eskisini sil
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(path) != null)
            AssetDatabase.DeleteAsset(path);

        AssetDatabase.CreateAsset(clip, path);
        
        // Player Controller'ı güncelle
        UpdateController("Assets/Character/Animations/Player.controller", clip);
        
        // Enemy Controller'ı güncelle
        UpdateController("Assets/Character/Animations/Enemy.controller", clip);

        AssetDatabase.SaveAssets();
        Debug.Log(">>> TEKME (KICK) ANIMASYONU BASARIYLA OLUSTURULDU VE ATANDI <<<");
    }

    static void UpdateController(string path, AnimationClip clip)
    {
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        if (controller == null) return;
        
        var rootStateMachine = controller.layers[0].stateMachine;
        foreach(var state in rootStateMachine.states) {
            if(state.state.name == "Punch") {
                state.state.motion = clip;
                state.state.speed = 1.2f; // Çok hızlı oynatmayalım ki kafa karıştırmasın
            }
        }
        EditorUtility.SetDirty(controller);
    }
}
