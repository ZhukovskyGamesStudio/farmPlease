using System;
using System.Globalization;
using Abstract;
using Cysharp.Threading.Tasks;
using Tables;
using UI;
using UnityEngine.SceneManagement;

namespace Managers {
    public class GameSceneEntryPoint : SceneEntryPoint {
        private void Awake() {
            if(ChangeToLoading.TryChangeScene()) {
                SceneManager.LoadScene("LoadingScene");
                return;
            }
        }

        protected override void Start() {
            Init();
            LoadGame();
            AdminManager.Instance.Init(SaveLoadManager.CurrentSave.IsAdmin);
            Settings.Instance.InitSettingsView();
            Clock.Instance.TryRefillForRealtimePassed();
            InventoryManager.Instance.CheckNewLevelDialog();
            KnowledgeHintsFactory.Instance.CheckAllUnshownHints();
            UIHud.Instance.UpdateLockedUI();
            UIHud.Instance.FarmerCommunityBadgeView.gameObject.SetActive(false);
            UIHud.Instance.OpenCroponomButton.UpdateGoldenState();
            UIHud.Instance.BatteryView.UpdateGoldenState();
            UIHud.Instance.FastPanelScript.UpdateGoldenScytheState();
            UIHud.Instance.ClockView.UpdateGoldenState();
            UIHud.Instance.ClockView.UpdateAdIcon();
            QuestsManager.Instance.TryStartQuestsTimer();
          
            if (KnowledgeUtils.HasKnowledge(Knowledge.Training)) {
                SmartTilemap.Instance.BrobotAnimTilemap.ShowIdle();
                QuestsUtils.ChangeTileView(SaveLoadManager.CurrentSave.QuestsData);

                UpdateDecorUpgradeState();
            } 
          
            
            if (!string.IsNullOrEmpty(SaveLoadManager.CurrentSave.UserId)) {
                FarmerCommunityManager.Instance.PreloadNextFarm().Forget();
            }
        }

        public static void UpdateDecorUpgradeState() {
            int decorUpgradeState = 1;
            if (UnlockableUtils.HasUnlockable(Unlockable.Field1.ToString())) {
                decorUpgradeState++;
            }

            if (UnlockableUtils.HasUnlockable(Unlockable.Field2.ToString())) {
                decorUpgradeState++;
            }

            AnimatedFarmBackground.Instance.SetUpgradeState(decorUpgradeState);
        }

        private static void Init() {
            PlayerController.Instance.Init();
            UIHud.Instance.FastPanelScript.Init();
            TilesTable.Instance.CreateDictionary();
        }

        private static void LoadGame() {
            if (GameModeManager.Instance.DoNotSave)
                SaveLoadManager.ClearSave();

          
            FirstSessionManager firstSessionManager = new();
            firstSessionManager.TryStartFtue();

            SetLoadedData();
        }

        private static void SetLoadedData() {
            GameSaveProfile save = SaveLoadManager.CurrentSave;
            Energy.Instance.SetEnergy(save.Energy);
            Clock.Instance.SetEnergy(save.ClockEnergy);

            SmartTilemap.Instance.GenerateTilesWithData(save.TilesData);

            InventoryManager.Instance.SetInventoryWithData();
            UIHud.Instance.FastPanelScript.UpdateToolsImages();

            // В этом методе запускается ежесекудный корутин, который подсчитывает кол-во прошедших дней.

            DateTime dateTime = DateTime.Parse(save.Date, CultureInfo.InvariantCulture);
            TimeManager.Instance.SetDaysWithData(save.Days, dateTime);
        }
    }
}