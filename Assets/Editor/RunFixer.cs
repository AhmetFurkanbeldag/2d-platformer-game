using UnityEditor;
public class RunFixer {
    [MenuItem("Tools/Run Kunai Fixer")]
    public static void Run() {
        KunaiFixer.FixKunai();
    }
}
