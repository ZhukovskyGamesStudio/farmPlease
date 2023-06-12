using System.Collections.Generic;
using Abstract;
using Managers;
using ScriptableObjects;
using Tables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Croponom : PreloadableSingleton<Croponom>, ISoundStarter {
        public FactsPage FactsPage;
        public Button GridButtonPrefab;

        [Header("Crops")] public GameObject CropsGrid;

        public Button CropsOpenButton;
        public CropsTable CropsTablePrefab;

        [Header("Weather")] public Button WeatherOpenButton;

        public GameObject WeatherGrid;
        public WeatherTable WeatherTablePrefab;

        [Header("Tools")] public Button ToolsOpenButton;

        public GameObject ToolsGrid;
        public ToolsTable ToolsTablePrefab;

        private List<Button> _cropsButtons;
        private List<Button> _toolButtons;
        private List<Button> _weatherButtons;

        [SerializeField] private GameObject Panel;

        /**********/

        protected override void OnFirstInit() {
            GenerateAllButtons();
        }

        public void Open() {
            Panel.SetActive(true);
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
            foreach (Button button in _cropsButtons) button.onClick.RemoveAllListeners();

            foreach (Button button in _weatherButtons) button.onClick.RemoveAllListeners();

            foreach (Button button in _toolButtons) button.onClick.RemoveAllListeners();
        }

        private void OpenPage(ConfigWithCroponomPage pageData) {
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
            _weatherButtons = new List<Button>();
            foreach (WeatherConfig weatherData in WeatherTablePrefab.WeathersSO) {
                Button weatherButton = Instantiate(GridButtonPrefab, WeatherGrid.transform);
                weatherButton.onClick.AddListener(() => FactsPage.UpdatePage(weatherData));

                weatherButton.GetComponent<Image>().sprite = weatherData.icon;
                _weatherButtons.Add(weatherButton);
            }
        }

        private void GenerateCropsButtons() {
            _cropsButtons = new List<Button>();
            foreach (CropConfig cropData in CropsTablePrefab.Crops) {
                Button cropButton = Instantiate(GridButtonPrefab, CropsGrid.transform);
                cropButton.onClick.AddListener(() => FactsPage.UpdatePage(cropData));

                cropButton.GetComponent<Image>().sprite = cropData.VegSprite;
                _cropsButtons.Add(cropButton);
            }
        }

        private void GenerateToolsButtons() {
            _toolButtons = new List<Button>();
            foreach (ToolConfig toolData in ToolsTablePrefab.ToolsSO) {
                Button toolButton = Instantiate(GridButtonPrefab, ToolsGrid.transform);
                toolButton.onClick.AddListener(() => FactsPage.UpdatePage(toolData));

                toolButton.GetComponent<Image>().sprite = toolData.FoodMarketSprite;
                _toolButtons.Add(toolButton);
            }
        }

        public new void PlaySound(int soundIndex) {
            Audio.Instance.PlaySound((Sounds) soundIndex);
        }
    }
}