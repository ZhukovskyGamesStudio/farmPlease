using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GameModeConfig", fileName = "GameModeConfig", order = 2)]
public class GameModeConfig : ScriptableObject {
    public bool ShowTileType;
    public bool DoNotSave;
    public bool DisableStrongWind;
    public bool IsBuildingsShopAlwaysOpen;

#if UNITY_EDITOR
    public bool IsSkipTraining;
#else
     public readonly bool IsSkipTraining = false;
#endif

    [Range(1f, 10)]
    public float GameSpeed = 1;
    

    
}