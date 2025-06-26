using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GameModeConfig", fileName = "GameModeConfig", order = 2)]
public class GameModeConfig : ScriptableObject {
    public bool UnlimitedMoneyCrops;
    public bool InfiniteEnergy;
    public bool InfiniteClockEnergy;
    public bool UnlimitedSeeds;
    public bool ShowTileType;
    public bool DoNotSave;
    public bool DisableStrongWind;
    public bool IsBuildingsShopAlwaysOpen;
    public bool IsSkipTraining;
    public bool Is10xXp = true;
  

    [Range(1f, 10)]
    public float GameSpeed = 1;
}