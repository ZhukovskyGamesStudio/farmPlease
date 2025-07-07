using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

namespace Managers {
    public class InventoryManager : Singleton<InventoryManager> {
        public static SerializableDictionary<Crop, bool> IsCropsBoughtD => SaveLoadManager.CurrentSave.IsCropsBoughtD;
        public static SerializableDictionary<ToolBuff, bool> IsToolsBoughtD => SaveLoadManager.CurrentSave.IsToolsBoughtD;
        public static SerializableDictionary<BuildingType, bool> IsBuildingsBoughtD => SaveLoadManager.CurrentSave.IsBuildingsBoughtD;

        public static SerializableDictionary<Crop, int> SeedsInventory => SaveLoadManager.CurrentSave.Seeds;
        public static SerializableDictionary<ToolBuff, int> ToolsActivated => SaveLoadManager.CurrentSave.ToolBuffs;
        public static SerializableDictionary<ToolBuff, int> ToolsStored => SaveLoadManager.CurrentSave.ToolBuffsStored;
        public static List<BuildingType> BuildingsStored => SaveLoadManager.CurrentSave.BuildingsStored;

        private bool _isUnlimitedFlag;

        private UIHud _uiHud => UIHud.Instance;
        private Backpack _backpack => UIHud.Instance.Backpack;
        private FastPanelScript _fastPanelScript => UIHud.Instance.FastPanelScript;

        /*****Сохранение и загрузка*****/

        public static void GenerateInventory() {
            SaveLoadManager.CurrentSave.Seeds = new SerializableDictionary<Crop, int>();
            SaveLoadManager.CurrentSave.ToolBuffs = new SerializableDictionary<ToolBuff, int>();

        
            foreach (CropConfig t in CropsTable.Instance.Crops) {
                SeedsInventory.Add(t.type, 0);
            }

            //создаём пустой словарь инструментов
            for (int i = 0; i < ToolsTable.Instance.ToolsSO.Length; i++) {
                ToolsActivated.Add(ToolsTable.Instance.ToolsSO[i].buff, 0);
                ToolsStored.Add(ToolsTable.Instance.ToolsSO[i].buff, 0);
            }

            GenerateIsBoughtData();
/*
            UpdateInventoryUI();
            _uiHud.SetCounters();
            _fastPanelScript.ChangeSeedFastPanel(Crop.Tomato, SeedsInventory[Crop.Tomato]);
            _fastPanelScript.UpdateToolsImages();*/
        }

        public void SetInventoryWithData() {
            _fastPanelScript.UpdateToolsImages();

            UpdateInventoryUI();

            _uiHud.SetCounters();
            _fastPanelScript.ChangeSeedFastPanel(Crop.Tomato, SeedsInventory[Crop.Tomato]);
        }

        private static void GenerateIsBoughtData() {
            SaveLoadManager.CurrentSave.IsCropsBoughtD = new SerializableDictionary<Crop, bool>();
            for (int i = 0; i < CropsTable.Instance.Crops.Length; i++) IsCropsBoughtD.Add((Crop)i, false);

            SaveLoadManager.CurrentSave.IsToolsBoughtD = new SerializableDictionary<ToolBuff, bool>();
            for (int i = 0; i < ToolsTable.Instance.ToolsSO.Length; i++) IsToolsBoughtD.Add((ToolBuff)i, false);

            SaveLoadManager.CurrentSave.IsBuildingsBoughtD = new SerializableDictionary<BuildingType, bool>();
            for (int i = 0; i < BuildingsTable.Instance.Buildings.Length; i++)
                IsBuildingsBoughtD.Add((BuildingType)i, false);
        }

        public void AddCollectedCrop(Crop crop, int amount) {
            for (int i = 0; i < amount; i++) {
                SaveLoadManager.CurrentSave.CropsCollected.Add(crop);
            }

            QuestsManager.TriggerQuest(QuestTypes.Collect.ToString() + TargetTypes.Crop, amount);
            QuestsManager.TriggerQuest(QuestTypes.Collect.ToString() + crop, amount);

            AddCropPoint(amount);
            UpdateInventoryUI();
        }

        public int CountCrops(Crop crop) {
            return SaveLoadManager.CurrentSave.CropsCollected.Count(c => c == crop);
        }

        public void AddCropPoint(int amount) {
            _uiHud.CountersView.CropsCounter.ChangeAmount(amount);
        }

        public void RemoveRandomCollectedCrops(int amount) {
            var list = SaveLoadManager.CurrentSave.CropsCollected;
            list = list.OrderBy((_) => Random.Range(0, 1f)).ToList();
            list.RemoveRange(0, amount);
            SaveLoadManager.CurrentSave.CropsCollected = list;
            AddCropPoint(-amount);
        }
        
        public bool HasEnoughCrops(int amount) {
            return SaveLoadManager.CurrentSave.CropsCollected.Count >= amount;
        }

        public void AddCoins(int amount) {
            SaveLoadManager.CurrentSave.Coins += amount;
            if (SaveLoadManager.CurrentSave.Coins < 0)
                SaveLoadManager.CurrentSave.Coins = 0;
            _uiHud.CountersView.CoinsCounter.ChangeAmount(amount);
        }

        public void AddXp(int amount) {
            if (SaveLoadManager.CurrentSave.RealShopData.HasGoldenCroponom) {
                _uiHud.OpenCroponomButton.SpawnAdditionalXp(amount);
                amount *= 2;
            }
          
            SaveLoadManager.CurrentSave.Xp += amount;
            UIHud.Instance.ProfileView.XpProgressBar.ChangeAmount(amount);
            CheckNewLevelDialog();
        }

        public static void CheckNewLevelDialog() {
            if (!XpUtils.IsNextLevel(SaveLoadManager.CurrentSave.CurrentLevel, SaveLoadManager.CurrentSave.Xp)) {
                return;
            }

            RewardWithUnlockable reward = ConfigsManager.Instance.LevelsConfig.LevelRewards[SaveLoadManager.CurrentSave.CurrentLevel].Reward;
            DialogsManager.Instance.ShowDialogWithData(typeof(NewLevelDialog), SaveLoadManager.CurrentSave.CurrentLevel+1);
            DialogsManager.Instance.ShowDialogWithData(typeof(RewardDialog), new RewardDialogData() {
                Reward = reward,
                OnClaim = () => {
                    SaveLoadManager.CurrentSave.CurrentLevel++;
                    UIHud.Instance.ProfileView.SetData(SaveLoadManager.CurrentSave);

                    if (SaveLoadManager.CurrentSave.CurrentLevel == ConfigsManager.Instance.CostsConfig.LevelToUnlockDaily) {
                        QuestsManager.Instance.GenerateSideQuests();
                        QuestsManager.Instance.TryStartQuestsTimer();
                    }
                    SaveLoadManager.SaveGame();
                }
            });
        }

        /*****Семена*****/

        public void ChooseSeed(Crop crop) {
            if (PlayerController.Instance.seedBagCrop != crop) {
                if (IsToolWorking(ToolBuff.Carpetseeder)) {
                    if (Energy.Instance.HasEnergy())
                        Energy.Instance.LoseOneEnergy();
                    else
                        return;
                }

                _fastPanelScript.ChangeSeedFastPanel(crop, SeedsInventory[crop]);
                PlayerController.Instance.seedBagCrop = crop;
            }
        }

        public bool TryBuySeed(Crop crop, int cost, int amount) {
            if (SaveLoadManager.CurrentSave.Coins >= cost) {
                Instance.AddXp(ConfigsManager.Instance.CostsConfig.XpForBuySeed);
                AddCoins(-1 * cost);
                AddSeed(crop, amount);
                StartCoroutine(SmartTilemap.Instance.HappeningSequence());
                return true;
            }

            return false;
        }

        public void AddSeed(Crop crop, int amount) {
            SeedsInventory[crop] += amount;
            UpdateInventoryUI();
            _fastPanelScript.UpdateSeedFastPanel(crop, SeedsInventory[crop]);

            TriggerCollectSeedTypes();
            SaveLoadManager.SaveGame();
        }

        public void RetriggerCollectionQuests() {
            TriggerCollectSeedTypes();
            TriggerCollectToolTypes();
        }

        private static void TriggerCollectToolTypes() {
            int diffToolTypes = ToolsStored.Values.Count(c => c > 0);
            QuestsManager.TriggerQuest(QuestTypes.Collect.ToString() + SpecialTargetTypes.CollectNToolTypes, diffToolTypes, true);
        }

        private static void TriggerCollectSeedTypes() {
            int diffSeddTypes = SeedsInventory.Values.Count(c => c > 0);
            QuestsManager.TriggerQuest(QuestTypes.Collect.ToString() + SpecialTargetTypes.CollectNSeedTypes, diffSeddTypes, true);
            
        }

        public void LoseSeed(Crop crop) {
            SeedsInventory[crop]--;

            UpdateInventoryUI();
            _fastPanelScript.UpdateSeedFastPanel(crop, SeedsInventory[crop]);
        }

        public IEnumerator WindyDay(SmartTilemap tilemap) {
            if (GameModeManager.Instance.DisableStrongWind)
                yield break;

            List<Crop> seedsList = new();
            foreach (var seedType in SeedsInventory.Keys) {
                int amount = SeedsInventory[seedType];
                for (int i = 0; i < amount; i++)
                    seedsList.Add(seedType);
            }

            SmartTile[] alltiles = tilemap.GetAllTiles();

            List<SmartTile> emptyTiles = new();

            for (int i = 0; i < alltiles.Length; i++)
                if (alltiles[i].CanBeSeeded())
                    emptyTiles.Add(alltiles[i]);

            int whileStopper = 1000;
            while (seedsList.Count > 0) {
                whileStopper--;
                if (whileStopper < 0) {
                    UnityEngine.Debug.Log("never use while!");
                    break;
                }

                Crop crop = seedsList[Random.Range(0, seedsList.Count)];
                seedsList.Remove(crop);
                LoseSeed(crop);

                if (emptyTiles.Count > 0) {
                    SmartTile tile = emptyTiles[Random.Range(0, emptyTiles.Count)];
                    emptyTiles.Remove(tile);
                    yield return StartCoroutine(tile.OnSeeded(crop, 0.2f));
                }

                yield return new WaitForSeconds(0.05f);
            }

            yield return false;
        }

        /*****Инструменты*****/

        public void BuyTool(ToolBuff buff, int cost, int amount) {
            AddCoins(-1 * cost);
            AddXp(ConfigsManager.Instance.CostsConfig.XpForBuyTool);
            AddTool(buff, amount);
        }

        public void AddTool(ToolBuff buff, int amount) {
            if (!ToolsStored.ContainsKey(buff)) {
                ToolsStored.Add(buff, 0);
            }
            QuestsManager.TriggerQuest(QuestTypes.Collect.ToString() + TargetTypes.Tool, amount);
            QuestsManager.TriggerQuest(QuestTypes.Collect.ToString() + buff, amount);
            ToolsStored[buff] += amount;
            TriggerCollectToolTypes();
            UpdateInventoryUI();
        }
        
        public void AddBuilding(BuildingType type) {
            if (!BuildingsStored.Contains(type)) {
                BuildingsStored.Add(type);
            }
            UpdateInventoryUI();
        }
        public void RemoveBuilding(BuildingType type) {
            if (BuildingsStored.Contains(type)) {
                BuildingsStored.Remove(type);
            }
            UpdateInventoryUI();
        }

        public void ActivateTool(ToolBuff buff) {
            ToolConfig config = ToolsTable.ToolByType(buff);
            if (buff == ToolBuff.WeekBattery && SaveLoadManager.CurrentSave.ClockEnergy > 0) {
                _uiHud.ClockView.TimeStillLeftAnimation();
                return;
            }

            if (!ToolsStored.ContainsKey(buff)) {
                ToolsStored.Add(buff, 0);
            }

            ToolsStored[buff]--;

            if (!ToolsActivated.ContainsKey(buff)) {
                ToolsActivated.Add(buff, 0);
            }

            if (config.IsInstant) {
                if (buff == ToolBuff.WeekBattery) {
                    Clock.Instance.RefillToMaxEnergy();
                }
            } else {
                ToolsActivated[buff] += config.workDaysAmount;
                _fastPanelScript.UpdateToolsImages();
            }

            switch (buff) {
                case ToolBuff.Unlimitedwatercan:
                    UIHud.Instance.FastPanelScript.ChangeTool((int)Tool.Watercan);
                    break;
                case ToolBuff.Doublehoe:
                    UIHud.Instance.FastPanelScript.ChangeTool((int)Tool.Hoe);
                    break;
                case ToolBuff.Carpetseeder:
                    UIHud.Instance.FastPanelScript.ChangeTool((int)Tool.SeedBag);
                    break;
                case ToolBuff.Greenscythe:
                    UIHud.Instance.FastPanelScript.ChangeTool((int)Tool.Collect);
                    break;
                case ToolBuff.Wetscythe:
                    UIHud.Instance.FastPanelScript.ChangeTool((int)Tool.Collect);
                    break;
            }
            
            
            SaveLoadManager.SaveGame();
            UpdateInventoryUI();
        }

        public void BrokeTools() {
            if (ToolsActivated == null) {
                UnityEngine.Debug.LogError("dictionary is null");
                return;
            }

            foreach (ToolBuff type in ToolsActivated.Keys.ToList()) {
                ToolsActivated[type]--;
                if (ToolsActivated[type] < 0)
                    ToolsActivated[type] = 0;
            }
        }

        public bool IsToolWorking(ToolBuff buff) {
            if (ToolsActivated.ContainsKey(buff))
                return ToolsActivated[buff] > 0;
            ToolsActivated.Add(buff, 0);
            return false;
        }

        public void BuyFoodMarket(Crop type, int cost) {
            IsCropsBoughtD[type] = true;
            TryBuySeed(type, 0, CropsTable.CropByType(type).buyAmount);
            UnlockableUtils.Unlock(type);
            RemoveRandomCollectedCrops(cost);
        }

        public void BuyFoodMarket(ToolBuff buff, int cost) {
            IsToolsBoughtD[buff] = true;
            BuyTool(buff, 0, ToolsTable.ToolByType(buff).buyAmount);
            UnlockableUtils.Unlock(buff);
            RemoveRandomCollectedCrops(cost);
        }

        public void BuyFoodMarket(BuildingType type, int cost) {
            IsBuildingsBoughtD[type] = true;
            AddBuilding(type);
            UnlockableUtils.Unlock(type);
            if (type == BuildingType.Sprinkler) {
                UnlockableUtils.Unlock(BuildingType.SprinklerTarget);
            }
            RemoveRandomCollectedCrops(cost);
        }

        public bool EnoughMoney(int cost) {
            return SaveLoadManager.CurrentSave.Coins >= cost;
        }

        public bool EnoughCrops(int cost) {
            return SaveLoadManager.CurrentSave.CropPoints >= cost;
        }

        public void UpdateInventoryUI() {
            _backpack.UpdateGrid();
        }
    }
}