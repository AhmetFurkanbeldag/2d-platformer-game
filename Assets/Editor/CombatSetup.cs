using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[InitializeOnLoad]
public class CombatSetup : Editor
{
    static CombatSetup()
    {
        EditorApplication.delayCall += Setup;
    }

    [MenuItem("Tools/Setup Combat")]
    public static void Setup()
    {
        string[] animators = { 
            "Assets/Character/Animations/Player.controller", 
            "Assets/Character/Animations/Enemy.controller"
        };

        foreach(var path in animators)
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if(controller != null)
            {
                bool hasParam = false;
                foreach(var p in controller.parameters) if(p.name == "punch") hasParam = true;
                if(!hasParam) controller.AddParameter("punch", AnimatorControllerParameterType.Trigger);

                var rootStateMachine = controller.layers[0].stateMachine;
                AnimatorState punchState = null;
                foreach(var state in rootStateMachine.states) if(state.state.name == "Punch") punchState = state.state;
                
                if(punchState == null)
                {
                    punchState = rootStateMachine.AddState("Punch");
                    // Hızlı tekme/yumruk hissi vermek için saldırı animasyonunu x2 hızında oynatıyoruz
                    punchState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Character/Animations/SamuraiAttack.anim");
                    punchState.speed = 2f; 

                    var transition = rootStateMachine.AddAnyStateTransition(punchState);
                    transition.AddCondition(AnimatorConditionMode.If, 0, "punch");
                    transition.duration = 0;
                    
                    var exitTransition = punchState.AddTransition(rootStateMachine.defaultState);
                    exitTransition.hasExitTime = true;
                    exitTransition.exitTime = 1f;
                    exitTransition.duration = 0;
                }
                
                EditorUtility.SetDirty(controller);
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log(">>> PUNCH (TEKME/YUMRUK) ANIMASYONLARI BASARIYLA EKLENDI <<<");
    }
}
