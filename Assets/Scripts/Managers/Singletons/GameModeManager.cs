using System;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace Managers
{
    public class GameModeManager : Singleton<GameModeManager> {
        public bool UnlimitedMoneyCrops;
        public bool InfiniteEnergy;
        public bool InfiniteClockEnergy;
        public bool UnlimitedSeeds;
        public bool ShowTileType;
        public bool DoNotSave;
        public bool DisableStrongWind;
        public bool IsBuildingsShopAlwaysOpen;
        [Range(1f, 5)] public float GameSpeed = 1;


        public GameMode GameMode = GameMode.FakeTime;

#if UNITY_EDITOR
        private void Update()
        {
            UnityEngine.Time.timeScale = GameSpeed;
        }
#endif
    }

    public enum GameMode {
        Training,
        RealTime,
        FakeTime,
        Online
    }
}