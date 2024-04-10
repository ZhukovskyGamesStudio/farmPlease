using Tables;
using UnityEngine;
using ZhukovskyGamesPlugin;

namespace Managers {
    public class GameModeManager : Singleton<GameModeManager> {
        [SerializeField]
        private GameModeConfig _gameModeConfig;

        public GameModeConfig Config => _gameModeConfig;
        
        public bool UnlimitedMoneyCrops => _gameModeConfig.UnlimitedMoneyCrops;
        public bool InfiniteEnergy => _gameModeConfig.InfiniteEnergy;
        public bool InfiniteClockEnergy => _gameModeConfig.InfiniteClockEnergy;
        public bool UnlimitedSeeds => _gameModeConfig.UnlimitedSeeds;
        public bool ShowTileType => _gameModeConfig.ShowTileType;
        public bool DoNotSave => _gameModeConfig.DoNotSave;
        public bool DisableStrongWind => _gameModeConfig.DisableStrongWind;
        public bool IsBuildingsShopAlwaysOpen => _gameModeConfig.IsBuildingsShopAlwaysOpen;
        public bool IsSkipTraining => _gameModeConfig.IsSkipTraining;
        public int RandomCropsCollectedQueueAmount => _gameModeConfig.RandomCropsCollectedQueueAmount;
        public float GameSpeed => _gameModeConfig.GameSpeed;

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