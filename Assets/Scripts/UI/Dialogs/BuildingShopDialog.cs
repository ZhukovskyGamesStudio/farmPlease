using System;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using Cysharp.Threading.Tasks;
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
    private FoodMarketConfirmDialog _confirmDialog;

    private Dictionary<BuildingType, FoodMarketOffer> _buildingButtonsD;
    private Dictionary<Crop, FoodMarketOffer> _cropButtonsD;
    private Dictionary<ToolBuff, FoodMarketOffer> _toolButtonsD;

    private Crop _lastSelectedCrop;
    private ToolBuff _lastSelectedTool;
    private BuildingType _lastSelectedBuilding;

    private BuildingShopData _data;

    [SerializeField]
    private List<GameObject> _tabs, _tabsNormal, _tabsPressed;

    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _idleClip, _buyClip;

    [SerializeField]
    private List<Color> _headerColors;

    private int ToolPrice => ConfigsManager.Instance.CostsConfig.ToolPrice;
    private int CropPrice => ConfigsManager.Instance.CostsConfig.CropPrice;
    private int BuildingPrice => ConfigsManager.Instance.CostsConfig.BuildingPriceProgression[_data.BuildingPriceIndex];

    public override void SetData(BuildingShopData data) {
        _data = data;

        GenerateButtons();
        UpdateButtonsInteractable();
        OpenCrops(true);
    }

    public void GenerateButtons() {
        GenerateBuildingsButtons();
        GenerateCropsButtons();
        GenerateToolsButtons();
    }

    public override UniTask Show(Action onClose) {
        _animation.Play(_idleClip.name);
        return base.Show(onClose);
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
            offer.Init(tools[i].FoodMarketSprite, delegate { OpenConfirmPage(tmpBuff); });

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
            offer.Init(crops[i].VegSprite, delegate { OpenConfirmPage(tmp); });

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
            offer.Init(buildings[i].offerSprite, delegate { OpenConfirmPage(type); });

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

        _lastSelectedCrop = _cropButtonsD.Keys.First();
        _lastSelectedTool = _toolButtonsD.Keys.First();
        _lastSelectedBuilding = _buildingButtonsD.Keys.First();
    
    }

    private void OpenConfirmPage(Crop type) {
        _lastSelectedCrop = type;
        CropConfig crop = CropsTable.CropByType(type);
        _confirmDialog.SetData(crop.VegSprite, LocalizationUtils.L(crop.HeaderLoc), LocalizationUtils.L(crop.explainTextLoc), CropPrice,
            delegate { BuyCropButton(type); },_headerColors[0]);
        _confirmDialog.UpdateInteractable(InventoryManager.IsCropsBoughtD[type],InventoryManager.Instance.EnoughCrops(CropPrice));
    }

    private void OpenConfirmPage(ToolBuff buff) {
        _lastSelectedTool = buff;
        ToolConfig tool = ToolsTable.ToolByType(buff);
        _confirmDialog.SetData(tool.FoodMarketSprite, LocalizationUtils.L(tool.HeaderLoc), LocalizationUtils.L(tool.explainTextLoc), ToolPrice,
            delegate { BuyToolButton(buff); },_headerColors[1]);
        _confirmDialog.UpdateInteractable(InventoryManager.IsToolsBoughtD[buff],InventoryManager.Instance.EnoughCrops(ToolPrice));
    }

    private void OpenConfirmPage(BuildingType type) {
        _lastSelectedBuilding = type;
        BuildingConfig building = BuildingsTable.BuildingByType(type);
        _confirmDialog.SetData(building.offerSprite, LocalizationUtils.L(building.HeaderLoc), LocalizationUtils.L(building.explainTextLoc),
            BuildingPrice, delegate { BuyBuildingButton(type); },_headerColors[2], true);
        _confirmDialog.UpdateInteractable(InventoryManager.IsBuildingsBoughtD[type],InventoryManager.Instance.EnoughCrops(BuildingPrice));
    }

    public void BuyCropButton(Crop type) {
        if (!InventoryManager.Instance.HasEnoughCrops(CropPrice)) {
            return;
        }

        InventoryManager.Instance.BuyFoodMarket(type, CropPrice);
        UpdateButtonsInteractable();
        UIHud.Instance.BackpackAttention.ShowAttention();
        _confirmDialog.UpdateInteractable(InventoryManager.IsCropsBoughtD[type],false);
        SaveLoadManager.SaveGame();
        ShowBuyAnimation();
    }

    public void BuyToolButton(ToolBuff buff) {
        if (!InventoryManager.Instance.HasEnoughCrops(ToolPrice)) {
            return;
        }

        InventoryManager.Instance.BuyFoodMarket(buff, ToolPrice);
        UIHud.Instance.BackpackAttention.ShowAttention();
        UpdateButtonsInteractable();
        _confirmDialog.UpdateInteractable(InventoryManager.IsToolsBoughtD[buff],false);
        SaveLoadManager.SaveGame();
        ShowBuyAnimation();
    }

    public void BuyBuildingButton(BuildingType type) {
        if (!InventoryManager.Instance.HasEnoughCrops(BuildingPrice)) {
            return;
        }

        InventoryManager.Instance.BuyFoodMarket(type, BuildingPrice);
        _data.BuildingPriceIndex++;
        UIHud.Instance.BackpackAttention.ShowAttention();
        UpdateButtonsInteractable();
        _confirmDialog.UpdateInteractable(InventoryManager.IsBuildingsBoughtD[type],false);
        UIHud.Instance.HammerToolButton.SetActive(true);
        SaveLoadManager.SaveGame();
        ShowBuyAnimation();
    }

    private void ShowBuyAnimation() {
        _animation.Play(_buyClip.name);
        _animation.PlayQueued(_idleClip.name);
    }

    public void OpenCrops(bool isOn) {
        if (!isOn) {
            return;
        }

        for (int index = 0; index < _tabsNormal.Count; index++) {
            _tabs[index].SetActive(index == 0);
            _tabsPressed[index].SetActive(index == 0);
            _tabsNormal[index].SetActive(index != 0);
        }

        OpenConfirmPage(_lastSelectedCrop);
    }

    public void OpenTools(bool isOn) {
        if (!isOn) {
            return;
        }

        for (int index = 0; index < _tabsNormal.Count; index++) {
            _tabs[index].SetActive(index == 1);
            _tabsPressed[index].SetActive(index == 1);
            _tabsNormal[index].SetActive(index != 1);
        }
        OpenConfirmPage(_lastSelectedTool);
    }

    public void OpenBuildings(bool isOn) {
        if (!isOn) {
            return;
        }

        for (int index = 0; index < _tabsNormal.Count; index++) {
            _tabs[index].SetActive(index == 2);
            _tabsPressed[index].SetActive(index == 2);
            _tabsNormal[index].SetActive(index != 2);
        }
        OpenConfirmPage(_lastSelectedBuilding);
    }
}

[System.Serializable]
public class BuildingShopData {
    public int BuildingPriceIndex;
}