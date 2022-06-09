using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encyclopedia : MonoBehaviourSoundStarter {
    public FactsPage FactsPage;
    public Button GridButtonPrefab;

    [Header("Crops")]
    public GameObject CropsGrid;

    public Button CropsOpenButton;
    public CropsTable CropsTablePrefab;

    [Header("Weather")]
    public Button WeatherOpenButton;

    public GameObject WeatherGrid;
    public WeatherTable WeatherTablePrefab;

    [Header("Tools")]
    public Button ToolsOpenButton;

    public GameObject ToolsGrid;
    public ToolsTable ToolsTablePrefab;

    private List<Button> cropsButtons;
    private List<Button> weatherButtons;
    private List<Button> toolButtons;

    /**********/
    private void Awake() {
        GenerateAllButtons();
        GridButtonPrefab.gameObject.SetActive(false);
    }

    private void SubscribeTabButtons() {
        CropsOpenButton.onClick.AddListener(() => OpenPage(CropsTablePrefab.Crops[0]));
        WeatherOpenButton.onClick.AddListener(() => OpenPage(WeatherTablePrefab.WeathersSO[0]));
        ToolsOpenButton.onClick.AddListener(() => OpenPage(ToolsTablePrefab.ToolsSO[0]));
    }

    private void ReleaseAllButtons() {
        CropsOpenButton.onClick.RemoveListener(() => OpenPage(CropsTablePrefab.Crops[0]));
        WeatherOpenButton.onClick.RemoveListener(() => OpenPage(WeatherTablePrefab.WeathersSO[0]));
        ToolsOpenButton.onClick.RemoveListener(() => OpenPage(ToolsTablePrefab.ToolsSO[0]));
        foreach (Button button in cropsButtons) {
            button.onClick.RemoveAllListeners();
        }

        foreach (Button button in weatherButtons) {
            button.onClick.RemoveAllListeners();
        }

        foreach (Button button in toolButtons) {
            button.onClick.RemoveAllListeners();
        }
    }

    private void OpenPage(SOWithCroponomPage pageData) {
        FactsPage.UpdatePage(pageData);
    }

    private void GenerateAllButtons() {
        GenerateCropsButtons();
        GenerateWeatherButtons();
        GenerateToolsButtons();

        SubscribeTabButtons();

        FactsPage.UpdatePage(CropsTablePrefab.Crops[0]);
    }

    private void GenerateWeatherButtons() {
        weatherButtons = new List<Button>();
        foreach (WeatherSO weatherData in WeatherTablePrefab.WeathersSO) {
            Button weatherButton = Instantiate(GridButtonPrefab, WeatherGrid.transform);
            weatherButton.onClick.AddListener(() => FactsPage.UpdatePage(weatherData));

            weatherButton.GetComponent<Image>().sprite = weatherData.icon;
            weatherButtons.Add(weatherButton);
        }
    }

    private void GenerateCropsButtons() {
        cropsButtons = new List<Button>();
        foreach (CropSO cropData in CropsTablePrefab.Crops) {
            Button cropButton = Instantiate(GridButtonPrefab, CropsGrid.transform);
            cropButton.onClick.AddListener(() => FactsPage.UpdatePage(cropData));

            cropButton.GetComponent<Image>().sprite = cropData.VegSprite;
            cropsButtons.Add(cropButton);
        }
    }

    private void GenerateToolsButtons() {
        toolButtons = new List<Button>();
        foreach (ToolSO toolData in ToolsTablePrefab.ToolsSO) {
            Button toolButton = Instantiate(GridButtonPrefab, ToolsGrid.transform);
            toolButton.onClick.AddListener(() => FactsPage.UpdatePage(toolData));

            toolButton.GetComponent<Image>().sprite = toolData.FoodMarketSprite;
            toolButtons.Add(toolButton);
        }
    }

    public new void PlaySound(int soundIndex) {
        AudioManager.instance.PlaySound((Sounds) soundIndex);
    }

    private void OnDestroy() {
        ReleaseAllButtons();
    }
}