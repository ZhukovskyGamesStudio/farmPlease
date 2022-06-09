using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingShopPanel : MonoBehaviourSoundStarter {
    public Text cropsCollectedText;

    [Header("Buildings")]
    public GameObject BuildingPanelButton;

    public int[] buildingPriceProgression;
    public int cropPrice;
    public int toolPrice;

    public Transform BuildingsGrid;
    public Transform ToolsGrid;
    public Transform CropsGrid;

    public GameObject BuildingPrefab;
    public GameObject ToolPrefab;
    public GameObject CropPrefab;

    [Header("Confirm Page")]
    public GameObject ConfirmPage;

    public Button ConfirmButton;
    public Text nameText;
    public Text explanationText;
    public Text costText;
    public Image confirmImage;
    private Dictionary<BuildingType, Button> buildingButtonsD;

    private Dictionary<CropsType, Button> cropButtonsD;

    private int currentBuildingPrice;
    private PlayerController player;
    private Dictionary<ToolType, Button> toolButtonsD;

    private void Start() {
        player = PlayerController.instance;
    }

    public void GenerateButtons() {
        //Buildings
        BuildingSO[] buildings = BuildingsTable.instance.Buildings;
        buildingButtonsD = new Dictionary<BuildingType, Button>();

        for (int i = 0; i < buildings.Length; i++) {
            if (buildings[i].IsFakeBuilding)
                continue;
            GameObject offerObject = Instantiate(BuildingPrefab, BuildingsGrid);
            offerObject.SetActive(true);
            FoodMarketOffer offer = offerObject.GetComponent<FoodMarketOffer>();
            offer.name.text = buildings[i].offerHeader;
            offer.image.sprite = buildings[i].offerSprite;
            BuildingType type = buildings[i].type;
            Button button = offer.GetComponent<Button>();
            buildingButtonsD.Add(type, button);

            button.onClick.AddListener(() => OpenConfirmPage(type));
        }

        CropSO[] crops = CropsTable.instance.Crops;
        cropButtonsD = new Dictionary<CropsType, Button>();

        for (int i = 0; i < crops.Length; i++) {
            if (crops[i].CanBeBought || crops[i].type == CropsType.Weed)
                continue;

            GameObject obj = Instantiate(CropPrefab, CropsGrid);
            obj.SetActive(true);
            Button button = obj.GetComponent<Button>();

            FoodMarketOffer offer = obj.GetComponent<FoodMarketOffer>();
            offer.name.text = crops[i].header;
            offer.image.sprite = crops[i].VegSprite;

            CropsType tmpType = crops[i].type;
            button.onClick.AddListener(() => OpenConfirmPage(tmpType));

            cropButtonsD.Add(tmpType, button);
        }

        ToolSO[] tools = ToolsTable.instance.ToolsSO;
        toolButtonsD = new Dictionary<ToolType, Button>();

        for (int i = 0; i < tools.Length; i++) {
            if (tools[i].isAlwaysAvailable)
                continue;

            GameObject obj = Instantiate(ToolPrefab, ToolsGrid);
            obj.SetActive(true);
            FoodMarketOffer offer = obj.GetComponent<FoodMarketOffer>();
            offer.name.text = tools[i].header;
            offer.image.sprite = tools[i].FoodMarketSprite;

            Button button = obj.GetComponent<Button>();

            ToolType tmpType = tools[i].type;
            button.onClick.AddListener(() => OpenConfirmPage(tmpType));
            toolButtonsD.Add(tmpType, button);
        }
    }

    public void Initialize() {
        currentBuildingPrice = 0;

        BuildingPanelButton.SetActive(false);
        GenerateButtons();
        UpdateButtonsInteractable();
    }

    public void InitializeWithData(int buildingPrice) {
        currentBuildingPrice = buildingPrice;

        //Если куплена хотя бы 1 постройка - то кнопка перестраивания построек становится активной
        BuildingPanelButton.SetActive(false);
        foreach (bool build in InventoryManager.instance.isBuildingsBoughtD.Values)
            if (build) {
                BuildingPanelButton.SetActive(true);
                break;
            }

        GenerateButtons();
        UpdateButtonsInteractable();
    }

    public int GetBuildingPrice() {
        return currentBuildingPrice;
    }

    public void UpdateButtonsInteractable() {
        foreach (CropsType item in cropButtonsD.Keys)
            if (InventoryManager.instance.isCropsBoughtD.ContainsKey(item))
                cropButtonsD[item].interactable = !InventoryManager.instance.isCropsBoughtD[item];
            else
                cropButtonsD[item].interactable = true;

        foreach (ToolType item in toolButtonsD.Keys)
            if (InventoryManager.instance.isToolsBoughtD.ContainsKey(item))
                toolButtonsD[item].interactable = !InventoryManager.instance.isToolsBoughtD[item];
            else
                toolButtonsD[item].interactable = true;

        foreach (BuildingType item in buildingButtonsD.Keys)
            if (InventoryManager.instance.isBuildingsBoughtD.ContainsKey(item))
                buildingButtonsD[item].interactable = !InventoryManager.instance.isBuildingsBoughtD[item];
            else
                buildingButtonsD[item].interactable = true;
    }

    public void UpdateCropsCollected() {
        cropsCollectedText.text = InventoryManager.instance.AllCropsCollected.ToString();
    }

    private void OpenConfirmPage(CropsType type) {
        ConfirmButton.onClick.RemoveAllListeners();
        if (InventoryManager.instance.EnoughCrops(cropPrice)) {
            ConfirmButton.onClick.AddListener(() => BuyCropButton(type));
            ConfirmButton.interactable = true;
        } else {
            ConfirmButton.interactable = false;
        }

        CropSO crop = CropsTable.CropByType(type);

        confirmImage.sprite = crop.VegSprite;
        nameText.text = crop.header;
        explanationText.text = crop.explainText;
        costText.text = "Открыть за " + cropPrice;

        ConfirmPage.SetActive(true);
    }

    private void OpenConfirmPage(ToolType type) {
        ConfirmButton.onClick.RemoveAllListeners();
        if (InventoryManager.instance.EnoughCrops(toolPrice)) {
            ConfirmButton.onClick.AddListener(() => BuyToolButton(type));
            ConfirmButton.interactable = true;
        } else {
            ConfirmButton.interactable = false;
        }

        ToolSO tool = ToolsTable.ToolByType(type);

        confirmImage.sprite = tool.FoodMarketSprite;
        nameText.text = tool.header;
        explanationText.text = tool.explainText;
        costText.text = "Открыть за " + toolPrice;

        ConfirmPage.SetActive(true);
    }

    private void OpenConfirmPage(BuildingType type) {
        ConfirmButton.onClick.RemoveAllListeners();
        if (InventoryManager.instance.EnoughCrops(buildingPriceProgression[currentBuildingPrice])) {
            ConfirmButton.onClick.AddListener(() => StartBuyingBuilding(type));
            ConfirmButton.interactable = true;
        } else {
            ConfirmButton.interactable = false;
        }

        BuildingSO building = BuildingsTable.BuildingByType(type);

        confirmImage.sprite = building.offerSprite;
        nameText.text = building.offerHeader;
        explanationText.text = building.offerText;
        costText.text = "Открыть за " + buildingPriceProgression[currentBuildingPrice];

        ConfirmPage.SetActive(true);
    }

    public void StartBuyingBuilding(BuildingType type) {
        player.StartStopBuilding();
        player.InitializeBuilding(type, buildingPriceProgression[currentBuildingPrice]);
        ConfirmPage.SetActive(false);
        gameObject.SetActive(false);
    }

    public void BuyCropButton(CropsType type) {
        InventoryManager.instance.BuyFoodMarket(type, cropPrice);
        UpdateButtonsInteractable();
        ConfirmPage.SetActive(false);
    }

    public void BuyToolButton(ToolType type) {
        InventoryManager.instance.BuyFoodMarket(type, toolPrice);

        UpdateButtonsInteractable();
        ConfirmPage.SetActive(false);
    }

    public void BuyBuildingButton(BuildingType type) {
        InventoryManager.instance.BuyFoodMarket(type, buildingPriceProgression[currentBuildingPrice]);
        currentBuildingPrice++;

        UpdateButtonsInteractable();
        ConfirmPage.SetActive(false);
        BuildingPanelButton.SetActive(true);
    }
}