using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager instance;
    public int startCoins = 5;
    public int coins;
    public int cropsCollected;
    public int AllCropsCollected;

    public Text[] seedsAmountText;

    private Backpack Backpack;
    public Queue<CropsType> cropsCollectedQueue;

    private FastPanelScript FastPanelScript;
    public Dictionary<BuildingType, bool> isBuildingsBoughtD;

    [Header("FoodMarketBought")]
    public Dictionary<CropsType, bool> isCropsBoughtD;

    public Dictionary<ToolType, bool> isToolsBoughtD;

    private bool IsUnlimitedFlag;
    private PlayerController PlayerController;
    public Dictionary<CropsType, int> seedsInventory;
    private TimePanel TimePanel;

    public Dictionary<ToolType, int> toolsInventory;

    private UIScript UIScript;

    /**********/

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update() {
        if (GameModeManager.instance.UnlimitedMoneyCrops && !IsUnlimitedFlag) {
            IsUnlimitedFlag = true;
            coins = 1000;
            AllCropsCollected = 100000;
            UIScript.ChangeCoins(coins);
        }
    }

    public void Init() {
        UIScript = UIScript.instance;
        PlayerController = PlayerController.instance;
        FastPanelScript = PlayerController.gameObject.GetComponent<FastPanelScript>();

        TimePanel = UIScript.TimePanel;
        Backpack = UIScript.Backpack;
    }

    /*****Сохранение и загрузка*****/

    public void GenerateInventory() {
        seedsInventory = new Dictionary<CropsType, int>();
        toolsInventory = new Dictionary<ToolType, int>();
        cropsCollectedQueue = new Queue<CropsType>();

        //создаём пустой словарь семян. заполняем его по 100, если включены читы
        for (int i = 0; i < CropsTable.instance.Crops.Length; i++)
            if (GameModeManager.instance.UnlimitedSeeds && CropsTable.instance.Crops[i].type != CropsType.Weed)
                seedsInventory.Add(CropsTable.instance.Crops[i].type, 100);
            else
                seedsInventory.Add(CropsTable.instance.Crops[i].type, 0);

        //создаём пустой словарь инструментов
        for (int i = 0; i < ToolsTable.instance.ToolsSO.Length; i++)
            toolsInventory.Add(ToolsTable.instance.ToolsSO[i].type, 0);

        if (GameModeManager.instance.UnlimitedMoneyCrops) {
            coins = 1000;
            cropsCollected = 1000;
        } else {
            coins = startCoins;
            cropsCollected = 0;
        }

        GenerateIsBoughtData();

        UpdateInventoryUI();
        UIScript.ChangeCoins(coins);
        FastPanelScript.ChangeSeedFastPanel(CropsType.Tomato, seedsInventory[CropsType.Tomato]);
        FastPanelScript.UpdateToolsImages();
    }

    public void SetInventoryWithData(string[] seeds, string[] crops, string[] tools, int coins, int allCrops,
        bool[] isCropBought, bool[] isToolBought, bool[] isBuildBought) {
        this.coins = coins;
        AllCropsCollected = allCrops;
        seedsInventory = new Dictionary<CropsType, int>();
        for (int i = 0; i < CropsTable.instance.Crops.Length; i++)
            seedsInventory.Add(CropsTable.instance.Crops[i].type, 0);
        if (seeds != null)
            for (int i = 0; i < seeds.Length; i++) {
                string[] parts = seeds[i].Split('_');
                CropsType type = (CropsType) Enum.Parse(typeof(CropsType), parts[0]);
                int amount = int.Parse(parts[1]);
                seedsInventory[type] = amount;
            }

        cropsCollectedQueue = new Queue<CropsType>();
        if (crops != null)
            for (int i = 0; i < crops.Length; i++)
                cropsCollectedQueue.Enqueue((CropsType) Enum.Parse(typeof(CropsType), crops[i]));

        cropsCollected = cropsCollectedQueue.Count;

        toolsInventory = new Dictionary<ToolType, int>();
        for (int i = 0; i < ToolsTable.instance.ToolsSO.Length; i++)
            toolsInventory.Add(ToolsTable.instance.ToolsSO[i].type, 0);

        if (tools != null)
            for (int i = 0; i < tools.Length; i++) {
                string[] parts = tools[i].Split('_');
                ToolType type = (ToolType) Enum.Parse(typeof(ToolType), parts[0]);
                int amount = int.Parse(parts[1]);
                toolsInventory[type] = amount;
            }

        SetIsBoughtData(0, isCropBought);
        SetIsBoughtData(1, isToolBought);
        SetIsBoughtData(2, isBuildBought);

        FastPanelScript.UpdateToolsImages();

        UpdateInventoryUI();

        UIScript.ChangeCoins(coins);
        FastPanelScript.ChangeSeedFastPanel(CropsType.Tomato, seedsInventory[CropsType.Tomato]);
    }

    public string[] GetSeedsData() {
        List<string> res = new();
        foreach (CropsType key in seedsInventory.Keys) res.Add(key + "_" + seedsInventory[key]);
        return res.ToArray();
    }

    public string[] GetToolsData() {
        List<string> res = new();
        foreach (ToolType key in toolsInventory.Keys) res.Add(key + "_" + toolsInventory[key]);
        return res.ToArray();
    }

    public string[] GetCollectedCropsData() {
        Queue<CropsType> queueToData;
        if (cropsCollectedQueue != null)
            queueToData = new Queue<CropsType>(cropsCollectedQueue);
        else
            queueToData = new Queue<CropsType>();

        string[] res = new string[queueToData.Count];
        for (int i = 0; i < res.Length; i++)
            res[i] = queueToData.Dequeue().ToString();

        return res;
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
                if (isToolsBoughtD.ContainsKey((ToolType) i))
                    res[i] = isToolsBoughtD[(ToolType) i];
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

        isToolsBoughtD = new Dictionary<ToolType, bool>();
        for (int i = 0; i < ToolsTable.instance.ToolsSO.Length; i++) isToolsBoughtD.Add((ToolType) i, false);

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
            isToolsBoughtD = new Dictionary<ToolType, bool>();
            for (int i = 0; i < ToolsTable.instance.ToolsSO.Length; i++)
                if (toSet != null && toSet.Length > i)
                    isToolsBoughtD.Add((ToolType) i, toSet[i]);
                else
                    isToolsBoughtD.Add((ToolType) i, false);
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
        for (int i = 0; i < amount; i++) cropsCollectedQueue.Enqueue(crop);
        cropsCollected += amount;
        AllCropsCollected += amount;
        UpdateInventoryUI();
        AddCoins(amount);
    }

    public void AddCoins(int amount) {
        coins += amount;
        if (coins < 0)
            coins = 0;
        UIScript.ChangeCoins(coins);
    }

    /*****Семена*****/

    public void ChooseSeed(CropsType crop) {
        if (PlayerController.seedBagCrop != crop) {
            if (IsToolWorking(ToolType.Carpetseeder)) {
                if (PlayerController.HasEnergy())
                    PlayerController.LoseOneEnergy();
                else
                    return;
            }

            FastPanelScript.ChangeSeedFastPanel(crop, seedsInventory[crop]);
            PlayerController.seedBagCrop = crop;
        }
    }

    public void BuySeed(CropsType crop, int cost, int amount) {
        if (coins >= cost) {
            seedsInventory[crop] += amount;
            AddCoins(-1 * cost);

            UpdateInventoryUI();
            FastPanelScript.UpdateSeedFastPanel(crop, seedsInventory[crop]);
            SaveLoadManager.instance.SaveGame();
        }
    }

    public void LoseSeed(CropsType crop) {
        seedsInventory[crop]--;

        UpdateInventoryUI();
        FastPanelScript.UpdateSeedFastPanel(crop, seedsInventory[crop]);
    }

    public IEnumerator WindyDay(SmartTilemap tilemap) {
        if (GameModeManager.instance.DisableStrongWind)
            yield break;

        List<CropsType> seedsList = new();
        foreach (KeyValuePair<CropsType, int> seeds in seedsInventory) {
            int amount = seeds.Value;
            for (int i = 0; i < amount; i++)
                seedsList.Add(seeds.Key);
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
                Debug.Log("never use while!");
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

    public void BuyTool(ToolType type, int cost, int amount) {
        if (!toolsInventory.ContainsKey(type))
            toolsInventory.Add(type, 0);

        AddCoins(-1 * cost);

        //Одна коса заменяет другую. Переделать в более универсальную систему
        if (type == ToolType.Wetscythe && toolsInventory.ContainsKey(ToolType.Greenscythe))
            toolsInventory[ToolType.Greenscythe] = 0;
        if (type == ToolType.Greenscythe && toolsInventory.ContainsKey(ToolType.Wetscythe))
            toolsInventory[ToolType.Wetscythe] = 0;
        toolsInventory[type] += amount;
        FastPanelScript.UpdateToolsImages();
    }

    public void BrokeTools() {
        if (toolsInventory == null) {
            Debug.LogError("dictionary is null");
            return;
        }

        foreach (ToolType type in toolsInventory.Keys.ToList()) {
            toolsInventory[type]--;
            if (toolsInventory[type] < 0)
                toolsInventory[type] = 0;
        }
    }

    public bool IsToolWorking(ToolType type) {
        if (toolsInventory.ContainsKey(type))
            return toolsInventory[type] > 0;
        toolsInventory.Add(type, 0);
        return false;
    }

    /**********/

    public void BuyFoodMarket(CropsType type, int cost) {
        isCropsBoughtD[type] = true;
        BuySeed(type, 0, CropsTable.CropByType(type).buyAmount);
        AllCropsCollected -= cost;
    }

    public void BuyFoodMarket(ToolType type, int cost) {
        isToolsBoughtD[type] = true;
        BuyTool(type, 0, ToolsTable.ToolByType(type).buyAmount);
        AllCropsCollected -= cost;
        FastPanelScript.UpdateToolsImages();
    }

    public void BuyFoodMarket(BuildingType type, int cost) {
        isBuildingsBoughtD[type] = true;
        AllCropsCollected -= cost;
    }

    /**********/

    public bool EnoughMoney(int cost) {
        return coins >= cost;
    }

    public bool EnoughCrops(int cost) {
        return AllCropsCollected >= cost;
    }

    public void UpdateInventoryUI() {
        Backpack.UpdateGrid(seedsInventory);
    }
}