using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Abstract;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager instance;
    public int startCoins = 5;

    public Text[] seedsAmountText;

    private Backpack Backpack;

    private FastPanelScript FastPanelScript;
    public Dictionary<BuildingType, bool> isBuildingsBoughtD;

    [Header("FoodMarketBought")]
    public Dictionary<CropsType, bool> isCropsBoughtD;

    public Dictionary<ToolBuff, bool> isToolsBoughtD;

    private bool IsUnlimitedFlag;
    private PlayerController PlayerController;
    public SerializableDictionary<CropsType, int> seedsInventory => SaveLoadManager.CurrentSave.Seeds;
    private TimePanel TimePanel;

    public SerializableDictionary<ToolBuff, int> toolsInventory => SaveLoadManager.CurrentSave.ToolBuffs;

    private UIHud _uiHud;

    /**********/

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update() {
        if (GameModeManager.Instance.UnlimitedMoneyCrops && !IsUnlimitedFlag) {
            IsUnlimitedFlag = true;
            SaveLoadManager.CurrentSave.Coins = 1000;
            SaveLoadManager.CurrentSave.CropPoints = 100000;
            _uiHud.ChangeCoins(SaveLoadManager.CurrentSave.Coins);
        }
    }

    public void Init() {
        _uiHud = UIHud.Instance;
        PlayerController = PlayerController.Instance;
        FastPanelScript = PlayerController.gameObject.GetComponent<FastPanelScript>();

        TimePanel = _uiHud.TimePanel;
        Backpack = _uiHud.Backpack;
    }

    /*****Сохранение и загрузка*****/

    public void GenerateInventory() {
        SaveLoadManager.CurrentSave.Seeds = new SerializableDictionary<CropsType, int>();
        SaveLoadManager.CurrentSave.ToolBuffs = new SerializableDictionary<ToolBuff, int>();

        //создаём пустой словарь семян. заполняем его по 100, если включены читы
        for (int i = 0; i < CropsTable.instance.Crops.Length; i++)
            if (GameModeManager.Instance.UnlimitedSeeds && CropsTable.instance.Crops[i].type != CropsType.Weed)
                seedsInventory.Add(CropsTable.instance.Crops[i].type, 100);
            else
                seedsInventory.Add(CropsTable.instance.Crops[i].type, 0);

        //создаём пустой словарь инструментов
        for (int i = 0; i < ToolsTable.instance.ToolsSO.Length; i++)
            toolsInventory.Add(ToolsTable.instance.ToolsSO[i].buff, 0);

        if (GameModeManager.Instance.UnlimitedMoneyCrops) {
            SaveLoadManager.CurrentSave.Coins = 1000;
            SaveLoadManager.CurrentSave.CropPoints = 1000;
        } else {
            SaveLoadManager.CurrentSave.Coins = startCoins;
            SaveLoadManager.CurrentSave.CropPoints = 0;
        }

        GenerateIsBoughtData();

        UpdateInventoryUI();
        _uiHud.ChangeCoins( SaveLoadManager.CurrentSave.Coins);
        FastPanelScript.ChangeSeedFastPanel(CropsType.Tomato, seedsInventory[CropsType.Tomato]);
        FastPanelScript.UpdateToolsImages();
    }

    public void SetInventoryWithData(GameSaveProfile save) {
        bool[] isCropBought = save.cropBoughtData;
        bool[] isToolBought = save.toolBoughtData;
        bool[] isBuildBought = save.buildingBoughtData;
        SetIsBoughtData(0, isCropBought);
        SetIsBoughtData(1, isToolBought);
        SetIsBoughtData(2, isBuildBought);

        FastPanelScript.UpdateToolsImages();

        UpdateInventoryUI();

        _uiHud.ChangeCoins(SaveLoadManager.CurrentSave.Coins);
        FastPanelScript.ChangeSeedFastPanel(CropsType.Tomato, seedsInventory[CropsType.Tomato]);
    }

    public bool[] GetIsBoughtData(int index) {
        bool[] res = null;
        if (index == 0) {
            res = new bool[CropsTable.instance.Crops.Length];
            for (int i = 0; i < res.Length; i++)
                if (isCropsBoughtD.ContainsKey((CropsType) i))
                    res[i] = isCropsBoughtD[(CropsType) i];
                else
                    res[i] = false;
        }

        if (index == 1) {
            res = new bool[ToolsTable.instance.ToolsSO.Length];
            for (int i = 0; i < res.Length; i++)
                if (isToolsBoughtD.ContainsKey((ToolBuff) i))
                    res[i] = isToolsBoughtD[(ToolBuff) i];
                else
                    res[i] = false;
        }

        if (index == 2) {
            res = new bool[BuildingsTable.instance.Buildings.Length];
            for (int i = 0; i < res.Length; i++)
                if (isBuildingsBoughtD.ContainsKey((BuildingType) i))
                    res[i] = isBuildingsBoughtD[(BuildingType) i];
                else
                    res[i] = false;
        }

        return res;
    }

    private void GenerateIsBoughtData() {
        isCropsBoughtD = new Dictionary<CropsType, bool>();
        for (int i = 0; i < CropsTable.instance.Crops.Length; i++) isCropsBoughtD.Add((CropsType) i, false);

        isToolsBoughtD = new Dictionary<ToolBuff, bool>();
        for (int i = 0; i < ToolsTable.instance.ToolsSO.Length; i++) isToolsBoughtD.Add((ToolBuff) i, false);

        isBuildingsBoughtD = new Dictionary<BuildingType, bool>();
        for (int i = 0; i < BuildingsTable.instance.Buildings.Length; i++)
            isBuildingsBoughtD.Add((BuildingType) i, false);
    }

    public void SetIsBoughtData(int index, bool[] toSet) {
        if (index == 0) {
            isCropsBoughtD = new Dictionary<CropsType, bool>();
            for (int i = 0; i < CropsTable.instance.Crops.Length; i++)
                if (toSet != null && toSet.Length > i)
                    isCropsBoughtD.Add((CropsType) i, toSet[i]);
                else
                    isCropsBoughtD.Add((CropsType) i, false);
        }

        if (index == 1) {
            isToolsBoughtD = new Dictionary<ToolBuff, bool>();
            for (int i = 0; i < ToolsTable.instance.ToolsSO.Length; i++)
                if (toSet != null && toSet.Length > i)
                    isToolsBoughtD.Add((ToolBuff) i, toSet[i]);
                else
                    isToolsBoughtD.Add((ToolBuff) i, false);
        }

        if (index == 2) {
            isBuildingsBoughtD = new Dictionary<BuildingType, bool>();
            for (int i = 0; i < BuildingsTable.instance.Buildings.Length; i++)
                if (toSet != null && toSet.Length > i)
                    isBuildingsBoughtD.Add((BuildingType) i, toSet[i]);
                else
                    isBuildingsBoughtD.Add((BuildingType) i, false);
        }
    }

    /**********/

    public void CollectCrop(CropsType crop, int amount) {
        for (int i = 0; i < amount; i++) SaveLoadManager.CurrentSave.CropsCollected.Enqueue(crop);
        SaveLoadManager.CurrentSave.CropPoints += amount;
        UpdateInventoryUI();
        AddCoins(amount);
    }

    public void AddCoins(int amount) {
        SaveLoadManager.CurrentSave.Coins += amount;
        if ( SaveLoadManager.CurrentSave.Coins < 0)
            SaveLoadManager.CurrentSave.Coins = 0;
        _uiHud.ChangeCoins( SaveLoadManager.CurrentSave.Coins);
    }

    /*****Семена*****/

    public void ChooseSeed(CropsType crop) {
        if (PlayerController.seedBagCrop != crop) {
            if (IsToolWorking(ToolBuff.Carpetseeder)) {
                if (Energy.Instance.HasEnergy())
                    Energy.Instance.LoseOneEnergy();
                else
                    return;
            }

            FastPanelScript.ChangeSeedFastPanel(crop, seedsInventory[crop]);
            PlayerController.seedBagCrop = crop;
        }
    }

    public void BuySeed(CropsType crop, int cost, int amount) {
        if ( SaveLoadManager.CurrentSave.Coins >= cost) {
            seedsInventory[crop] += amount;
            AddCoins(-1 * cost);

            UpdateInventoryUI();
            FastPanelScript.UpdateSeedFastPanel(crop, seedsInventory[crop]);
            SaveLoadManager.Instance.SaveGame();
        }
    }

    public void LoseSeed(CropsType crop) {
        seedsInventory[crop]--;

        UpdateInventoryUI();
        FastPanelScript.UpdateSeedFastPanel(crop, seedsInventory[crop]);
    }

    public IEnumerator WindyDay(SmartTilemap tilemap) {
        if (GameModeManager.Instance.DisableStrongWind)
            yield break;

        List<CropsType> seedsList = new();
        foreach (var seedType in seedsInventory.Keys) {
            int amount = seedsInventory[seedType];
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

            CropsType crop = seedsList[Random.Range(0, seedsList.Count)];
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
        if (!toolsInventory.ContainsKey(buff))
            toolsInventory.Add(buff, 0);

        AddCoins(-1 * cost);

        //Одна коса заменяет другую. Переделать в более универсальную систему
        if (buff == ToolBuff.Wetscythe && toolsInventory.ContainsKey(ToolBuff.Greenscythe))
            toolsInventory[ToolBuff.Greenscythe] = 0;
        if (buff == ToolBuff.Greenscythe && toolsInventory.ContainsKey(ToolBuff.Wetscythe))
            toolsInventory[ToolBuff.Wetscythe] = 0;
        toolsInventory[buff] += amount;
        FastPanelScript.UpdateToolsImages();
    }

    public void BrokeTools() {
        if (toolsInventory == null) {
            UnityEngine.Debug.LogError("dictionary is null");
            return;
        }

        foreach (ToolBuff type in toolsInventory.Keys.ToList()) {
            toolsInventory[type]--;
            if (toolsInventory[type] < 0)
                toolsInventory[type] = 0;
        }
    }

    public bool IsToolWorking(ToolBuff buff) {
        if (toolsInventory.ContainsKey(buff))
            return toolsInventory[buff] > 0;
        toolsInventory.Add(buff, 0);
        return false;
    }

    /**********/

    public void BuyFoodMarket(CropsType type, int cost) {
        isCropsBoughtD[type] = true;
        BuySeed(type, 0, CropsTable.CropByType(type).buyAmount);
        SaveLoadManager.CurrentSave.CropPoints -= cost;
    }

    public void BuyFoodMarket(ToolBuff buff, int cost) {
        isToolsBoughtD[buff] = true;
        BuyTool(buff, 0, ToolsTable.ToolByType(buff).buyAmount);
        SaveLoadManager.CurrentSave.CropPoints -= cost;
        FastPanelScript.UpdateToolsImages();
    }

    public void BuyFoodMarket(BuildingType type, int cost) {
        isBuildingsBoughtD[type] = true;
        SaveLoadManager.CurrentSave.CropPoints -= cost;
    }

    /**********/

    public bool EnoughMoney(int cost) {
        return  SaveLoadManager.CurrentSave.Coins >= cost;
    }

    public bool EnoughCrops(int cost) {
        return  SaveLoadManager.CurrentSave.CropPoints >= cost;
    }

    public void UpdateInventoryUI() {
        Backpack.UpdateGrid(seedsInventory);
    }
}