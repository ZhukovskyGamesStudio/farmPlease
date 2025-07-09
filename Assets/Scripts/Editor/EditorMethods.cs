using Managers;
using UnityEditor;
using UnityEngine;

public class EditorMethods : MonoBehaviour {
#if UNITY_EDITOR
    [MenuItem("FarmPlease Tools/ClearSave")]
    private static void ClearSave() {
        SaveLoadManager.ClearSave();
    }

    [MenuItem("FarmPlease Tools/InvertSkipTraining")]
    private static void InvertSkipTraining() {
        var config = AssetDatabase.LoadAssetAtPath<GameModeConfig>("Assets/Configs/GameModeConfig.asset");
        config.IsSkipTraining = !config.IsSkipTraining;
        Debug.Log(config.IsSkipTraining ? "Skip training is ON" : "Skip training is OFF");
    }
#endif
}