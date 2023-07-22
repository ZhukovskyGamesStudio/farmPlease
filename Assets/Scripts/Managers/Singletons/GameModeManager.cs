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
        public int RandomCropsCollectedQueueAmount = 55;
        [Range(1f, 5)] public float GameSpeed = 1;


        public GameMode GameMode = GameMode.FakeTime;
        private KeyboardManager _keyboardManager;

        protected override void OnFirstInit() {
            base.OnFirstInit();
            _keyboardManager = new KeyboardManager();
        }

#if UNITY_EDITOR
        private void Update() {
            UnityEngine.Time.timeScale = GameSpeed;
            _keyboardManager.CheckInputs();
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