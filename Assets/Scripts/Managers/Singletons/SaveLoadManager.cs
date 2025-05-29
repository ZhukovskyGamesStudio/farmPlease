using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

namespace Managers {
    public class SaveLoadManager : Singleton<SaveLoadManager> {

        private BuildingShopView _buildingShopView;
        private FastPanelScript _fastPanelScript;

        private InventoryManager _inventoryManager;
        private PlayerController _playerController;
        private SeedShopView _seedShop;
        private SmartTilemap _smartTilemap;

        private ToolShopView _toolShop;
        public static GameSaveProfile CurrentSave;

        // Пока в игре происходят какие-то действия, игрок не может ничего сделать
        // По окончанию этих действий игрок снова может что-то делать, а игра сохраняется. Если последовательность не была завершена - то игра не сохранится и откатится назад при след. загрузке
        public void Sequence(bool isStart) {
            if (isStart) {
                PlayerController.CanInteract = false;
            } else {
                PlayerController.CanInteract = true;
                SaveGame();
            }
        }

        public static string GenerateJsonString() {
            CurrentSave.SavedDate = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture);

            CurrentSave.TilesData = SmartTilemap.Instance.GetTilesData();

            UIHud.Instance.ShopsPanel.seedShopView.GetButtonsData(out Crop first, out Crop second);
            CurrentSave.ShopFirstOffer = first;
            CurrentSave.ShopSecondOffer = second;

            CurrentSave.SeedShopChangeButton = UIHud.Instance.ShopsPanel.seedShopView.ChangeSeedsButton.activeSelf;

            CurrentSave.ToolShopChangeButton = UIHud.Instance.ShopsPanel.toolShopView.ChangeButton.activeSelf;

            if (GameModeManager.Instance.GameMode != GameMode.Training) {
                CurrentSave.BuildingPrice = UIHud.Instance.ShopsPanel.BuildingShopView.GetBuildingPrice();
            }

            return JsonUtility.ToJson(CurrentSave, false);
        }

        public static void SaveGame() {
            string jsonData = GenerateJsonString();
            PlayerPrefs.SetString("saveProfile", jsonData);
        }
        
        public static void LoadGame(string jsonString = null) {
            if (PlayerPrefs.HasKey("saveProfile")) {
                string jsonData = PlayerPrefs.GetString("saveProfile");

                CurrentSave = JsonUtility.FromJson<GameSaveProfile>(jsonData);
                TryUpdateSave();
            } else {
                GenerateGame();
                Debug.Instance.Log("Generating finished. Saving started");
                SaveGame();
                Debug.Instance.Log("New profile is saved");
            }

            TryAddAdminCropsCollectedQueue();
        }

        private static void TryUpdateSave() {
            UpdateKnowledge();
            UpdateTools();
            if (!KnowledgeUtils.HasKnowledge(Knowledge.Training)) {
                GenerateGame();
                Debug.Instance.Log("Generating finished. Saving started");
                SaveGame();
                Debug.Instance.Log("New profile is saved");
            }
            //TODO update everything else and move to another manager
        }
        private static void UpdateKnowledge() {
            bool isPlayedEnough = CurrentSave.CropPoints > 3 || CurrentSave.Coins > 5 || CurrentSave.CurrentDay > 2;
            if (!KnowledgeUtils.HasKnowledge(Knowledge.Training) && isPlayedEnough) {
                KnowledgeUtils.AddKnowledge(Knowledge.Training);
            }

            if (!KnowledgeUtils.HasKnowledge(Knowledge.Weather) && CurrentSave.CurrentDay > 3) {
                KnowledgeUtils.AddKnowledge(Knowledge.Weather);
            }
        }

        private static void UpdateTools() {
            if (CurrentSave.ToolBuffs.Count < ToolsTable.Tools.Count) {
                foreach (var buff in ToolsTable.Tools) {
                    if (!CurrentSave.ToolBuffs.ContainsKey(buff)) {
                        CurrentSave.ToolBuffs.Add(buff, 0);
                    }
                }
            }

            foreach (ToolBuff buff in CurrentSave.ToolBuffsStored.Keys) {
                if (CurrentSave.ToolBuffsStored[buff] < 0) {
                    CurrentSave.ToolBuffsStored[buff] = 0;
                }
            }
        }

        private static void TryAddAdminCropsCollectedQueue() {
#if UNITY_EDITOR
            if (GameModeManager.Instance.RandomCropsCollectedQueueAmount > 0) {
                CurrentSave.CropsCollected = new List<Crop>();
                int cropTypesAmount = Enum.GetValues(typeof(Crop)).Length - 1;
                for (int i = 0; i < GameModeManager.Instance.RandomCropsCollectedQueueAmount; i++) {
                    CurrentSave.CropsCollected.Add((Crop)Random.Range(0, cropTypesAmount));
                }
            }
#endif
        }

        public static void GenerateGame() {
            CurrentSave = new GameSaveProfile() {
                Coins = 3,
                CropPoints = 0,
                SavedDate = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture),
                Date = TimeManager.FirstDayOfGame.ToString(CultureInfo.InvariantCulture),
                AmbarCrop = Crop.None
            };

            SmartTilemap.GenerateTiles();
            InventoryManager.GenerateInventory();

            Energy.GenerateEnergy();
            Clock.GenerateEnergy();
            UIHud.Instance.ShopsPanel.seedShopView.ChangeSeedsNewDay();
            UIHud.Instance.ShopsPanel.toolShopView.ChangeToolsNewDay();
            TimeManager.GenerateDays(0);
        }

        public void ClearSaveAndReload() {
            ClearSave();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void ClearSave() {
            PlayerPrefs.DeleteKey("saveProfile");
        }

#if UNITY_EDITOR
        private void OnApplicationQuit() {
            if (GameModeManager.Instance.DoNotSave) {
                ClearSave();
            }
        }
#endif
    }
}