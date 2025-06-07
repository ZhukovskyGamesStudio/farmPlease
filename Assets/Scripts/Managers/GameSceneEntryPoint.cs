using System;
using System.Globalization;
using Abstract;
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
            InventoryManager.CheckNewLevelDialog();
            KnowledgeHintsFactory.Instance.CheckAllUnshownHints();
            UIHud.Instance.DisableLockedUI();
        }

        private static void Init() {
            PlayerController.Instance.Init();
            UIHud.Instance.FastPanelScript.Init();
            TilesTable.Instance.CreateDictionary();
        }

        private static void LoadGame() {
            if (GameModeManager.Instance.DoNotSave)
                SaveLoadManager.ClearSave();

            SaveLoadManager.LoadGame();
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

            UIHud.Instance.ShopsPanel.BuildingShopView.InitializeWithData(save.BuildingPrice);

            // В этом методе запускается ежесекудный корутин, который подсчитывает кол-во прошедших дней.

            DateTime dateTime = DateTime.Parse(save.Date, CultureInfo.InvariantCulture);
            TimeManager.Instance.SetDaysWithData(save.Days, dateTime);
        }
    }
}