using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

namespace Managers
{
    public class InventoryManager : Singleton<InventoryManager> {
        

        private Backpack _backpack;
        private FastPanelScript _fastPanelScript;

        [Header("FoodMarketBought")]
        public Dictionary<Crop, bool> IsCropsBoughtD;

        public Dictionary<ToolBuff, bool> IsToolsBoughtD;
        public Dictionary<BuildingType, bool> IsBuildingsBoughtD;

        private bool _isUnlimitedFlag;
        public SerializableDictionary<Crop, int> SeedsInventory => SaveLoadManager.CurrentSave.Seeds;
        public SerializableDictionary<ToolBuff, int> ToolsActivated => SaveLoadManager.CurrentSave.ToolBuffs;
        public SerializableDictionary<ToolBuff, int> ToolsStored => SaveLoadManager.CurrentSave.ToolBuffsStored;

        private UIHud _uiHud;

        /**********/

        private void Update() {
            if (GameModeManager.Instance.UnlimitedMoneyCrops && !_isUnlimitedFlag) {
                _isUnlimitedFlag = true;
                SaveLoadManager.CurrentSave.Coins = 99999;
                SaveLoadManager.CurrentSave.CropPoints = 99999;
                _uiHud.SetCounters();
            }
        }

        public void Init() {
            _uiHud = UIHud.Instance;

            _fastPanelScript = UIHud.Instance.FastPanelScript;

            _backpack = _uiHud.Backpack;
        }

        /*****Сохранение и загрузка*****/

        public void GenerateInventory() {
            SaveLoadManager.CurrentSave.Seeds = new SerializableDictionary<Crop, int>();
            SaveLoadManager.CurrentSave.ToolBuffs = new SerializableDictionary<ToolBuff, int>();

            //создаём пустой словарь семян. заполняем его по 100, если включены читы
            for (int i = 0; i < CropsTable.Instance.Crops.Length; i++)
                if (GameModeManager.Instance.UnlimitedSeeds && CropsTable.Instance.Crops[i].type != Crop.Weed)
                    SeedsInventory.Add(CropsTable.Instance.Crops[i].type, 100);
                else
                    SeedsInventory.Add(CropsTable.Instance.Crops[i].type, 0);

            //создаём пустой словарь инструментов
            for (int i = 0; i < ToolsTable.Instance.ToolsSO.Length; i++) {
                ToolsActivated.Add(ToolsTable.Instance.ToolsSO[i].buff, 0);
                ToolsStored.Add(ToolsTable.Instance.ToolsSO[i].buff, 0);
            }
               

            if (GameModeManager.Instance.UnlimitedMoneyCrops) {
                SaveLoadManager.CurrentSave.Coins = 1000;
                SaveLoadManager.CurrentSave.CropPoints = 1000;
            }

            GenerateIsBoughtData();

            UpdateInventoryUI();
            _uiHud.SetCounters();
            _fastPanelScript.ChangeSeedFastPanel(Crop.Tomato, SeedsInventory[Crop.Tomato]);
            _fastPanelScript.UpdateToolsImages();
        }

        public void SetInventoryWithData(GameSaveProfile save) {
            bool[] isCropBought = save.CropBoughtData;
            bool[] isToolBought = save.ToolBoughtData;
            bool[] isBuildBought = save.BuildingBoughtData;
            SetIsBoughtData(0, isCropBought);
            SetIsBoughtData(1, isToolBought);
            SetIsBoughtData(2, isBuildBought);

            _fastPanelScript.UpdateToolsImages();

            UpdateInventoryUI();

            _uiHud.SetCounters();
            _fastPanelScript.ChangeSeedFastPanel(Crop.Tomato, SeedsInventory[Crop.Tomato]);
        }

        public bool[] GetIsBoughtData(int index) {
            bool[] res = null;
            if (index == 0) {
                res = new bool[CropsTable.Instance.Crops.Length];
                for (int i = 0; i < res.Length; i++)
                    if (IsCropsBoughtD.ContainsKey((Crop) i))
                        res[i] = IsCropsBoughtD[(Crop) i];
                    else
                        res[i] = false;
            }

            if (index == 1) {
                res = new bool[ToolsTable.Instance.ToolsSO.Length];
                for (int i = 0; i < res.Length; i++)
                    if (IsToolsBoughtD.ContainsKey((ToolBuff) i))
                        res[i] = IsToolsBoughtD[(ToolBuff) i];
                    else
                        res[i] = false;
            }

            if (index == 2) {
                res = new bool[BuildingsTable.Instance.Buildings.Length];
                for (int i = 0; i < res.Length; i++)
                    if (IsBuildingsBoughtD.ContainsKey((BuildingType) i))
                        res[i] = IsBuildingsBoughtD[(BuildingType) i];
                    else
                        res[i] = false;
            }

            return res;
        }

        private void GenerateIsBoughtData() {
            IsCropsBoughtD = new Dictionary<Crop, bool>();
            for (int i = 0; i < CropsTable.Instance.Crops.Length; i++) IsCropsBoughtD.Add((Crop) i, false);

            IsToolsBoughtD = new Dictionary<ToolBuff, bool>();
            for (int i = 0; i < ToolsTable.Instance.ToolsSO.Length; i++) IsToolsBoughtD.Add((ToolBuff) i, false);

            IsBuildingsBoughtD = new Dictionary<BuildingType, bool>();
            for (int i = 0; i < BuildingsTable.Instance.Buildings.Length; i++)
                IsBuildingsBoughtD.Add((BuildingType) i, false);
        }

        public void SetIsBoughtData(int index, bool[] toSet) {
            if (index == 0) {
                IsCropsBoughtD = new Dictionary<Crop, bool>();
                for (int i = 0; i < CropsTable.Instance.Crops.Length; i++)
                    if (toSet != null && toSet.Length > i)
                        IsCropsBoughtD.Add((Crop) i, toSet[i]);
                    else
                        IsCropsBoughtD.Add((Crop) i, false);
            }

            if (index == 1) {
                IsToolsBoughtD = new Dictionary<ToolBuff, bool>();
                for (int i = 0; i < ToolsTable.Instance.ToolsSO.Length; i++)
                    if (toSet != null && toSet.Length > i)
                        IsToolsBoughtD.Add((ToolBuff) i, toSet[i]);
                    else
                        IsToolsBoughtD.Add((ToolBuff) i, false);
            }

            if (index == 2) {
                IsBuildingsBoughtD = new Dictionary<BuildingType, bool>();
                for (int i = 0; i < BuildingsTable.Instance.Buildings.Length; i++)
                    if (toSet != null && toSet.Length > i)
                        IsBuildingsBoughtD.Add((BuildingType) i, toSet[i]);
                    else
                        IsBuildingsBoughtD.Add((BuildingType) i, false);
            }
        }

        /**********/

        public void AddCollectedCrop(Crop crop, int amount) {
            for (int i = 0; i < amount; i++) SaveLoadManager.CurrentSave.CropsCollected.Add(crop);
            AddCropPoint(amount);
            UpdateInventoryUI();
        }

        public void AddCropPoint(int amount) {
            SaveLoadManager.CurrentSave.CropPoints += amount;
            if (SaveLoadManager.CurrentSave.CropPoints < 0)
                SaveLoadManager.CurrentSave.CropPoints = 0;
            _uiHud.CountersView.CropsCounter.ChangeAmount(amount);
        }

        public void AddCoins(int amount) {
            SaveLoadManager.CurrentSave.Coins += amount;
            if (SaveLoadManager.CurrentSave.Coins < 0)
                SaveLoadManager.CurrentSave.Coins = 0;
            _uiHud.CountersView.CoinsCounter.ChangeAmount(amount);
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

        public void BuySeed(Crop crop, int cost, int amount) {
            if (SaveLoadManager.CurrentSave.Coins >= cost) {
                SeedsInventory[crop] += amount;
                AddCoins(-1 * cost);
                UpdateInventoryUI();
                _fastPanelScript.UpdateSeedFastPanel(crop, SeedsInventory[crop]);
                SaveLoadManager.SaveGame();
                StartCoroutine(SmartTilemap.Instance.HappeningSequence());
            }
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
            if (!ToolsStored.ContainsKey(buff))
                ToolsStored.Add(buff, 0);

            AddCoins(-1 * cost);

            ToolsStored[buff] +=amount;
            UpdateInventoryUI();
        }
        
        public void ActivateTool(ToolBuff buff) {
            if (!ToolsStored.ContainsKey(buff)) {
                ToolsStored.Add(buff, 0);
            }

            ToolsStored[buff]--;

            if (!ToolsActivated.ContainsKey(buff)) {
                ToolsActivated.Add(buff, 0);
            }

            ToolConfig config = ToolsTable.ToolByType(buff);
           

            if (config.IsInstant ) {
                if (buff == ToolBuff.WeekBattery) {
                    Clock.Instance.RefillToMaxEnergy();
                }
            } else {
                ToolsActivated[buff]+= config.buyAmount;
                _fastPanelScript.UpdateToolsImages();
            }
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

        /**********/

        public void BuyFoodMarket(Crop type, int cost) {
            IsCropsBoughtD[type] = true;
            BuySeed(type, 0, CropsTable.CropByType(type).buyAmount);
            AddCropPoint(-cost);
        }

        public void BuyFoodMarket(ToolBuff buff, int cost) {
            IsToolsBoughtD[buff] = true;
            BuyTool(buff, 0, ToolsTable.ToolByType(buff).buyAmount);
            AddCropPoint(-cost);
        }

        public void BuyFoodMarket(BuildingType type, int cost) {
            IsBuildingsBoughtD[type] = true;
            AddCropPoint(-cost);
        }

        /**********/

        public bool EnoughMoney(int cost) {
            return SaveLoadManager.CurrentSave.Coins >= cost;
        }

        public bool EnoughCrops(int cost) {
            return SaveLoadManager.CurrentSave.CropPoints >= cost;
        }

        public void UpdateInventoryUI() {
            _backpack.UpdateGrid(SeedsInventory);
        }
    }
}