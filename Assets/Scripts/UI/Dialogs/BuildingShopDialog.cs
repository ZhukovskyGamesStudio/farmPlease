using System.Collections.Generic;
using Abstract;
using Localization;
using Managers;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;

public class BuildingShopDialog : DialogWithData<BuildingShopData>, ISoundStarter {
    [Header("Buildings")]
    public Transform BuildingsGrid;

    public Transform ToolsGrid;
    public Transform CropsGrid;

    public FoodMarketOffer BuildingPrefab;
    public FoodMarketOffer ToolPrefab;
    public FoodMarketOffer CropPrefab;

    [Header("Confirm Page")]
    [SerializeField]
    private GameObject _confirmPage;

    [SerializeField]
    private GameObject _notChoosedPage;

    [SerializeField]
    private FoodMarketConfirmDialog _confirmDialog;

    private Dictionary<BuildingType, FoodMarketOffer> _buildingButtonsD;
    private Dictionary<Crop, FoodMarketOffer> _cropButtonsD;
    private Dictionary<ToolBuff, FoodMarketOffer> _toolButtonsD;

    private BuildingShopData _data;

    private int ToolPrice => ConfigsManager.Instance.CostsConfig.ToolPrice;
    private int CropPrice => ConfigsManager.Instance.CostsConfig.CropPrice;
    private int BuildingPrice => ConfigsManager.Instance.CostsConfig.BuildingPriceProgression[_data.BuildingPriceIndex];

    public override void SetData(BuildingShopData data) {
        _data = data;

        GenerateButtons();
        UpdateButtonsInteractable();
    }

    public void GenerateButtons() {
        GenerateBuildingsButtons();
        GenerateCropsButtons();
        GenerateToolsButtons();
    }

    private void GenerateToolsButtons() {
        ToolConfig[] tools = ToolsTable.Instance.ToolsSO;
        _toolButtonsD = new Dictionary<ToolBuff, FoodMarketOffer>();

        for (int i = 0; i < tools.Length; i++) {
            if (tools[i].isAlwaysAvailable)
                continue;

            ToolBuff tmpBuff = tools[i].buff;
            FoodMarketOffer offer = Instantiate(ToolPrefab, ToolsGrid);
            offer.gameObject.SetActive(true);
            offer.Init(tools[i].FoodMarketSprite, LocalizationUtils.L(tools[i].HeaderLoc), delegate { OpenConfirmPage(tmpBuff); });

            _toolButtonsD.Add(tmpBuff, offer);
        }
    }

    private void GenerateCropsButtons() {
        CropConfig[] crops = CropsTable.Instance.Crops;
        _cropButtonsD = new Dictionary<Crop, FoodMarketOffer>();

        for (int i = 0; i < crops.Length; i++) {
            if (crops[i].CanBeBought || crops[i].type == Crop.Weed)
                continue;

            FoodMarketOffer offer = Instantiate(CropPrefab, CropsGrid);
            offer.gameObject.SetActive(true);
            Crop tmp = crops[i].type;
            offer.Init(crops[i].VegSprite, LocalizationUtils.L(crops[i].HeaderLoc), delegate { OpenConfirmPage(tmp); });

            _cropButtonsD.Add(tmp, offer);
        }
    }

    private void GenerateBuildingsButtons() {
        BuildingConfig[] buildings = BuildingsTable.Instance.Buildings;
        _buildingButtonsD = new Dictionary<BuildingType, FoodMarketOffer>();

        for (int i = 0; i < buildings.Length; i++) {
            if (buildings[i].IsFakeBuilding)
                continue;
            FoodMarketOffer offer = Instantiate(BuildingPrefab, BuildingsGrid);
            offer.gameObject.SetActive(true);
            BuildingType type = buildings[i].type;
            offer.Init(buildings[i].offerSprite, LocalizationUtils.L(buildings[i].HeaderLoc), delegate { OpenConfirmPage(type); });

            _buildingButtonsD.Add(type, offer);
        }
    }

    public void UpdateButtonsInteractable() {
        foreach (Crop item in _cropButtonsD.Keys)
            if (InventoryManager.IsCropsBoughtD.ContainsKey(item))
                _cropButtonsD[item].UpdateInteractable(!InventoryManager.IsCropsBoughtD[item]);
            else
                _cropButtonsD[item].UpdateInteractable(true);

        foreach (ToolBuff item in _toolButtonsD.Keys)
            if (InventoryManager.IsToolsBoughtD.ContainsKey(item))
                _toolButtonsD[item].UpdateInteractable(!InventoryManager.IsToolsBoughtD[item]);
            else
                _toolButtonsD[item].UpdateInteractable(true);

        foreach (BuildingType item in _buildingButtonsD.Keys)
            if (InventoryManager.IsBuildingsBoughtD.ContainsKey(item))
                _buildingButtonsD[item].UpdateInteractable(!InventoryManager.IsBuildingsBoughtD[item]);
            else
                _buildingButtonsD[item].UpdateInteractable(true);
    }

    private void OpenConfirmPage(Crop type) {
        CropConfig crop = CropsTable.CropByType(type);
        _confirmDialog.SetData(crop.VegSprite, LocalizationUtils.L(crop.HeaderLoc), LocalizationUtils.L(crop.explainTextLoc), CropPrice, delegate { BuyCropButton(type); });
        _confirmDialog.UpdateInteractable(InventoryManager.Instance.EnoughCrops(CropPrice));

        SetConfirmPageActive(true);
    }

    private void OpenConfirmPage(ToolBuff buff) {
        ToolConfig tool = ToolsTable.ToolByType(buff);
        _confirmDialog.SetData(tool.FoodMarketSprite, LocalizationUtils.L(tool.HeaderLoc), LocalizationUtils.L(tool.explainTextLoc), ToolPrice, delegate { BuyToolButton(buff); });
        _confirmDialog.UpdateInteractable(InventoryManager.Instance.EnoughCrops(ToolPrice));

        SetConfirmPageActive(true);
    }

    private void OpenConfirmPage(BuildingType type) {
        BuildingConfig building = BuildingsTable.BuildingByType(type);
        _confirmDialog.SetData(building.offerSprite, LocalizationUtils.L(building.HeaderLoc), LocalizationUtils.L(building.explainTextLoc), BuildingPrice,
            delegate { BuyBuildingButton(type); }, true);
        _confirmDialog.UpdateInteractable(InventoryManager.Instance.EnoughCrops(BuildingPrice));

        SetConfirmPageActive(true);
    }

    public void BuyCropButton(Crop type) {
        if (!InventoryManager.Instance.HasEnoughCrops(CropPrice)) {
            return;
        }

        InventoryManager.Instance.BuyFoodMarket(type, CropPrice);
        UpdateButtonsInteractable();
        UIHud.Instance.BackpackAttention.ShowAttention();
        SetConfirmPageActive(false);
        SaveLoadManager.SaveGame();
    }

    public void BuyToolButton(ToolBuff buff) {
        if (!InventoryManager.Instance.HasEnoughCrops(ToolPrice)) {
            return;
        }

        InventoryManager.Instance.BuyFoodMarket(buff, ToolPrice);
        UIHud.Instance.BackpackAttention.ShowAttention();
        UpdateButtonsInteractable();
        SetConfirmPageActive(false);
        SaveLoadManager.SaveGame();
    }

    public void BuyBuildingButton(BuildingType type) {
        if (!InventoryManager.Instance.HasEnoughCrops(BuildingPrice)) {
            return;
        }

        InventoryManager.Instance.BuyFoodMarket(type, BuildingPrice);
        _data.BuildingPriceIndex++;
        UIHud.Instance.BackpackAttention.ShowAttention();
        UpdateButtonsInteractable();
        SetConfirmPageActive(false);
        UIHud.Instance.HammerToolButton.SetActive(true);
        SaveLoadManager.SaveGame();
    }

    public void SetConfirmPageActive(bool isActive) {
        _confirmPage.SetActive(isActive);
        _notChoosedPage.SetActive(!isActive);
    }
}

[System.Serializable]
public class BuildingShopData {
    public int BuildingPriceIndex;
}