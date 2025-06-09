using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        private SeedShopDialog _seedShop;
        private SmartTilemap _smartTilemap;

        private ToolShopDialog _toolShop;
        public static GameSaveProfile CurrentSave;
        private static bool _needToSave;
        private static bool _isWaitingSequence;

        // Пока в игре происходят какие-то действия, игрок не может ничего сделать
        // По окончанию этих действий игрок снова может что-то делать, а игра сохраняется. Если последовательность не была завершена - то игра не сохранится и откатится назад при след. загрузке

        private List<string> _activeSequences = new List<string>();
        
        public string StartSequence() {
            _isWaitingSequence = true;
            PlayerController.CanInteract = false;
            string sequenceId = DateTime.Now.ToString(CultureInfo.InvariantCulture) + Random.Range(0, 1f);
            _activeSequences.Add(sequenceId);
            return sequenceId;
        }
        
        public void EndSequence(string sequenceId) {
            _activeSequences.Remove(sequenceId);
            if (_activeSequences.Count == 0) {
                _isWaitingSequence = false;
                PlayerController.CanInteract = true;
                SaveGame(); 
            }
        }

        public static string GenerateJsonString() {
            CurrentSave.SavedDate = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture);

            if (GameModeManager.Instance.GameMode != GameMode.Training) {
                CurrentSave.BuildingPrice = UIHud.Instance.ShopsPanel.BuildingShopView.GetBuildingPrice();
            }

            return JsonUtility.ToJson(CurrentSave, false);
        }

        public static void SaveGame() {
            _needToSave = true;
        }

        private static void RewriteGameSavedData() {
            string jsonData = GenerateJsonString();
            PlayerPrefs.SetString("saveProfile", jsonData);
        }

        private void LateUpdate() {
            if (_isWaitingSequence) {
                return;
            }

            if (_needToSave) {
                _needToSave = false;
                RewriteGameSavedData();
                if (!string.IsNullOrEmpty(CurrentSave.UserId)) {
                    PlayerAPI.UpdatePlayerAsync(CurrentSave.UserId, CurrentSave).Forget();
                }
            }
        }

        public static void LoadGame(string jsonString = null) {
            if (PlayerPrefs.HasKey("saveProfile")) {
                string jsonData = PlayerPrefs.GetString("saveProfile");

                CurrentSave = JsonUtility.FromJson<GameSaveProfile>(jsonData);
                TryUpdateSave();
            } else {
                GenerateGame();
                Debug.Instance.Log("Generating finished. Saving started");
                RewriteGameSavedData();
                Debug.Instance.Log("New profile is saved");
            }

            TryAddAdminCropsCollectedQueue();
        }

        private static void TryUpdateSave() {
            UpdateTools();
            if (!KnowledgeUtils.HasKnowledge(Knowledge.Training)) {
                GenerateGame();
                Debug.Instance.Log("Generating finished. Saving started");
                RewriteGameSavedData();
                Debug.Instance.Log("New profile is saved");
            }

            if (CurrentSave.Unlocked == null) {
                CurrentSave.Unlocked = UnlockableUtils.GetInitialUnlockables();
            } else {
                CurrentSave.Unlocked.AddRange(UnlockableUtils.GetInitialUnlockables());
                CurrentSave.Unlocked = CurrentSave.Unlocked.Distinct().ToList();
            }

            if (string.IsNullOrEmpty(CurrentSave.Nickname)) {
                CurrentSave.Nickname = "Farmer #" + Random.Range(999, 10000);
            }

            UnlockableUtils.TryRemoveSeenPage(Unlockable.ToolShop.ToString());
            for (int i = 0; i < CurrentSave.CurrentLevel; i++) {
                var unlockable = ConfigsManager.Instance.LevelConfigs[i].Reward.Unlockable;
                if (!CurrentSave.Unlocked.Contains(unlockable)) {
                    UnlockableUtils.Unlock(unlockable);
                }
            }
            
            
            if (string.IsNullOrEmpty(CurrentSave.UserId)) {
                CreatePlayerOnServer().Forget();
            } else {
                FindPlayerOnServer().Forget();
            }
            //TODO update everything else and move to another manager
        }

        private static async UniTask CreatePlayerOnServer() {
            string createdId = await PlayerAPI.CreatePlayerAsync(CurrentSave);
            if (createdId != null) {
                CurrentSave.UserId = createdId;
                SaveGame();
            }
        }

        private static async UniTaskVoid FindPlayerOnServer() {
            var res = await PlayerAPI.GetPlayerAsync(CurrentSave.UserId);
            if (res == null) {
                await CreatePlayerOnServer();
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

        private static void GenerateGame() {
            CurrentSave = new GameSaveProfile() {
                Coins = 0,
                SavedDate = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture),
                Date = TimeManager.FirstDayOfGame.ToString(CultureInfo.InvariantCulture),
                AmbarCrop = Crop.None,
                Unlocked = UnlockableUtils.GetInitialUnlockables(),
                TilesData = SmartTilemap.GenerateFtueTiles(),
                Nickname = "Farmer #" + Random.Range(999, 10000),
            };

            InventoryManager.GenerateInventory();

            Energy.GenerateEnergy();
            Clock.GenerateEnergy();
            TimeManager.GenerateDays(CurrentSave.ParsedDate);
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