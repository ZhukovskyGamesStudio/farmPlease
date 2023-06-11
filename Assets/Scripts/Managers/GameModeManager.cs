using DefaultNamespace.Abstract;
using UnityEngine;

public class GameModeManager : Singleton<GameModeManager> {
    public bool UnlimitedMoneyCrops;
    public bool InfiniteEnergy;
    public bool InfiniteClockEnergy;
    public bool UnlimitedSeeds;
    public bool ShowTileType;
    public bool DoNotSave;
    public bool DisableStrongWind;
    public bool IsBuildingsShopAlwaysOpen;

    public GameMode GameMode = GameMode.FakeTime;
}

public enum GameMode {
    Training,
    RealTime,
    FakeTime,
    Online
}