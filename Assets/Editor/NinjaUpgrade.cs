using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Tilemaps;
using System.IO;

[InitializeOnLoad]
public class NinjaUpgrade : Editor
{
    static NinjaUpgrade()
    {
        EditorApplication.delayCall += DoUpgrade;
    }

    [MenuItem("Tools/Ninja and Level Upgrade")]
    public static void DoUpgrade()
    {
        // 1. Dövüş animasyonunu (EnemyAttack.anim) oluştur
        string animPath = "Assets/Character/Animations/EnemyAttack.anim";
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
        if (clip == null)
        {
            clip = new AnimationClip();
            clip.frameRate = 12;
            
            EditorCurveBinding spriteBinding = new EditorCurveBinding();
            spriteBinding.type = typeof(SpriteRenderer);
            spriteBinding.path = "";
            spriteBinding.propertyName = "m_Sprite";

            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[3];
            for (int i = 0; i < 3; i++)
            {
                keyFrames[i] = new ObjectReferenceKeyframe();
                keyFrames[i].time = i / 12f;
                // ninja sprite klasöründen attack_0, attack_1, attack_2 frame'leri
                keyFrames[i].value = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Character/Sprites/Ninja/attack_{i}.png");
            }
            
            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames);
            AssetDatabase.CreateAsset(clip, animPath);
        }

        // 2. Animator Controller'a ekle
        string controllerPath = "Assets/Character/Animations/Enemy.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller != null)
        {
            var rootStateMachine = controller.layers[0].stateMachine;
            
            AnimatorState attackState = null;
            foreach (var state in rootStateMachine.states)
            {
                if (state.state.name == "EnemyAttack")
                {
                    attackState = state.state;
                    break;
                }
            }

            if (attackState == null)
            {
                attackState = rootStateMachine.AddState("EnemyAttack");
                attackState.motion = clip;

                // "attack" trigger'ı var mı kontrol et
                bool hasTrigger = false;
                foreach (var param in controller.parameters)
                {
                    if (param.name == "attack") hasTrigger = true;
                }
                if (!hasTrigger) controller.AddParameter("attack", AnimatorControllerParameterType.Trigger);

                // AnyState -> Attack geçişi
                var transition = rootStateMachine.AddAnyStateTransition(attackState);
                transition.AddCondition(AnimatorConditionMode.If, 0, "attack");
                transition.duration = 0;
                transition.hasExitTime = false;

                // Attack -> EnemyIdle geçişi (Animasyon bitince bekleme moduna dönsün)
                AnimatorState idleState = null;
                foreach (var state in rootStateMachine.states)
                {
                    if (state.state.name == "EnemyIdle" || state.state.name == "Idle")
                        idleState = state.state;
                }

                if (idleState != null)
                {
                    var exitTransition = attackState.AddTransition(idleState);
                    exitTransition.hasExitTime = true;
                    exitTransition.exitTime = 1f;
                    exitTransition.duration = 0f;
                }
            }
        }

        // 3. Bölümdeki platformları/tilemap'i arka plana (gece temasına) uyumlu hale getir
        var tilemaps = GameObject.FindObjectsOfType<Tilemap>();
        // Geleneksel Japon gece köyü temasına uygun koyu lacivert/mor tonu
        Color nightTint = new Color(0.45f, 0.5f, 0.7f, 1f); 
        
        foreach (var tm in tilemaps)
        {
            tm.color = nightTint;
            EditorUtility.SetDirty(tm);
        }

        // Değişiklikleri sahneye kaydet
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log(">>> NINJA ANIMASYONLARI VE TILEMAP RENKLERI BASARIYLA GUNCELLENDI <<<");
    }
}
