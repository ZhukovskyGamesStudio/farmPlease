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
    public int RandomCropsCollectedQueueAmount = 55;

    [Range(1f, 5)]
    public float GameSpeed = 1;
}