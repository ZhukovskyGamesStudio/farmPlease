using System.Collections.Generic;
using Abstract;
using Managers;
using ScriptableObjects;
using Tables;
using UnityEngine;

namespace UI {
    public class BuildingShopView : MonoBehaviour, ISoundStarter {
        [Header("Buildings")]
        public GameObject BuildingPanelButton;

        public int[] buildingPriceProgression;
        public int cropPrice;
        public int toolPrice;

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

        private int _currentBuildingPrice;
        private Dictionary<ToolBuff, FoodMarketOffer> _toolButtonsD;

        public void InitializeWithData(int buildingPrice) {
            _currentBuildingPrice = buildingPrice;

            //Если куплена хотя бы 1 постройка - то кнопка перестраивания построек становится активной
            BuildingPanelButton.SetActive(false);
            foreach (bool build in InventoryManager.Instance.IsBuildingsBoughtD.Values)
                if (build) {
                    BuildingPanelButton.SetActive(true);
                    break;
                }

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
                offer.Init(tools[i].FoodMarketSprite, tools[i].header, delegate { OpenConfirmPage(tmpBuff); });

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
                offer.Init(crops[i].VegSprite, crops[i].header, delegate { OpenConfirmPage(tmp); });

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
                offer.Init(buildings[i].offerSprite, buildings[i].offerHeader, delegate { OpenConfirmPage(type); });

                _buildingButtonsD.Add(type, offer);
            }
        }

        public int GetBuildingPrice() {
            return _currentBuildingPrice;
        }

        public void UpdateButtonsInteractable() {
            foreach (Crop item in _cropButtonsD.Keys)
                if (InventoryManager.Instance.IsCropsBoughtD.ContainsKey(item))
                    _cropButtonsD[item].UpdateInteractable(!InventoryManager.Instance.IsCropsBoughtD[item]);
                else
                    _cropButtonsD[item].UpdateInteractable(true);

            foreach (ToolBuff item in _toolButtonsD.Keys)
                if (InventoryManager.Instance.IsToolsBoughtD.ContainsKey(item))
                    _toolButtonsD[item].UpdateInteractable(!InventoryManager.Instance.IsToolsBoughtD[item]);
                else
                    _toolButtonsD[item].UpdateInteractable(true);

            foreach (BuildingType item in _buildingButtonsD.Keys)
                if (InventoryManager.Instance.IsBuildingsBoughtD.ContainsKey(item))
                    _buildingButtonsD[item].UpdateInteractable(!InventoryManager.Instance.IsBuildingsBoughtD[item]);
                else
                    _buildingButtonsD[item].UpdateInteractable(true);
        }

        private void OpenConfirmPage(Crop type) {
            CropConfig crop = CropsTable.CropByType(type);
            _confirmDialog.SetData(crop.VegSprite, crop.header, crop.explainText, cropPrice, delegate { BuyCropButton(type); });
            _confirmDialog.UpdateInteractable(InventoryManager.Instance.EnoughCrops(cropPrice));

            SetConfirmPageActive(true);
        }

        private void OpenConfirmPage(ToolBuff buff) {
            ToolConfig tool = ToolsTable.ToolByType(buff);
            _confirmDialog.SetData(tool.FoodMarketSprite, tool.header, tool.explainText, toolPrice, delegate { BuyToolButton(buff); });
            _confirmDialog.UpdateInteractable(InventoryManager.Instance.EnoughCrops(toolPrice));

            SetConfirmPageActive(true);
        }

        private void OpenConfirmPage(BuildingType type) {
            BuildingConfig building = BuildingsTable.BuildingByType(type);
            int buildingCost = buildingPriceProgression[_currentBuildingPrice];
            _confirmDialog.SetData(building.offerSprite, building.offerHeader, building.offerText, buildingCost,
                delegate { StartBuyingBuilding(type); });
            _confirmDialog.UpdateInteractable(InventoryManager.Instance.EnoughCrops(buildingCost));

            SetConfirmPageActive(true);
        }

        public void StartBuyingBuilding(BuildingType type) {
            PlayerController.Instance.StartStopBuilding();
            PlayerController.Instance.InitializeBuilding(type, buildingPriceProgression[_currentBuildingPrice]);
            SetConfirmPageActive(false);
            gameObject.SetActive(false);
        }

        public void BuyCropButton(Crop type) {
            InventoryManager.Instance.BuyFoodMarket(type, cropPrice);
            UpdateButtonsInteractable();
            SetConfirmPageActive(false);
        }

        public void BuyToolButton(ToolBuff buff) {
            InventoryManager.Instance.BuyFoodMarket(buff, toolPrice);

            UpdateButtonsInteractable();
            SetConfirmPageActive(false);
        }

        public void BuyBuildingButton(BuildingType type) {
            InventoryManager.Instance.BuyFoodMarket(type, buildingPriceProgression[_currentBuildingPrice]);
            _currentBuildingPrice++;

            UpdateButtonsInteractable();
            SetConfirmPageActive(false);
            BuildingPanelButton.SetActive(true);
        }

        public void SetConfirmPageActive(bool isActive) {
            _confirmPage.SetActive(isActive);
            _notChoosedPage.SetActive(!isActive);
        }
    }
}