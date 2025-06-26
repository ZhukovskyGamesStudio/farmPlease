using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Abstract;
using Cysharp.Threading.Tasks;
using Tables;
using UnityEngine;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

namespace Managers {
    public class SaveLoadManager : PreloadableSingleton<SaveLoadManager> {
        public static GameSaveProfile CurrentSave;
        private static bool _needToSave;
        private static bool _isWaitingSequence;

        // Пока в игре происходят какие-то действия, игрок не может ничего сделать
        // По окончанию этих действий игрок снова может что-то делать, а игра сохраняется. Если последовательность не была завершена - то игра не сохранится и откатится назад при след. загрузке

        private List<string> _activeSequences = new List<string>();
        
        public override int InitPriority => -1000;

        protected override void OnFirstInit() {
            base.OnFirstInit();
            LoadGame();
        }

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

                if (KnowledgeUtils.HasKnowledge(Knowledge.Training)) {
                    if (!string.IsNullOrEmpty(CurrentSave.UserId)) {
                        PlayerAPI.UpdatePlayerAsync(CurrentSave.UserId, CurrentSave).Forget();
                    }
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

            foreach (var toSkip in UnlockableUtils.NotInCroponom) {
                UnlockableUtils.TryRemoveSeenPage(toSkip);
            }
           
            for (int i = 0; i < CurrentSave.CurrentLevel; i++) {
                var unlockable = ConfigsManager.Instance.LevelConfigs[i].Reward.Unlockable;
                if (!CurrentSave.Unlocked.Contains(unlockable)) {
                    UnlockableUtils.Unlock(unlockable);
                }
            }

            MigrationUtils.TryMigrateToBuildingShopData(CurrentSave);
            MigrationUtils.TryMigrateToQuestsData(CurrentSave);

            if (KnowledgeUtils.HasKnowledge(Knowledge.Training)) {
                if (string.IsNullOrEmpty(CurrentSave.UserId)) {
                    CreatePlayerOnServer().Forget();
                } else {
                    FindPlayerOnServer().Forget();
                }
            }
           
            //TODO update everything else and move to another manager
        }

        public static void TryCreateFirstSave() {
            if (string.IsNullOrEmpty(CurrentSave.UserId)&& KnowledgeUtils.HasKnowledge(Knowledge.Training)) {
                CreatePlayerOnServer().Forget();
            }
        }

        private static async UniTask CreatePlayerOnServer() {
            string createdId = await PlayerAPI.CreatePlayerAsync(CurrentSave);
            if (!string.IsNullOrEmpty(createdId)) {
                CurrentSave.UserId = createdId;
                SaveGame();
            }
        }

        private static async UniTaskVoid FindPlayerOnServer() {
            var res = await PlayerAPI.GetPlayerAsync(CurrentSave.UserId);
            if (res == null) {
                UnityEngine.Debug.Log("❌ Player not found on server, creating new player");
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

        private static void GenerateGame() {
            CurrentSave = new GameSaveProfile() {
                Coins = 0,
                SavedDate = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture),
                Date = TimeManager.FirstDayOfGame.ToString(CultureInfo.InvariantCulture),
                Unlocked = UnlockableUtils.GetInitialUnlockables(),
                TilesData = SmartTilemap.GenerateFtueTiles(),
                Nickname = "Farmer #" + Random.Range(999, 10000),
            };

            InventoryManager.GenerateInventory();

            Energy.GenerateEnergy();
            Clock.GenerateEnergy();
            TimeManager.GenerateDays(CurrentSave.ParsedDate);
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