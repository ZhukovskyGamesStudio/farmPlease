using System;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using Managers;
using ScriptableObjects;
using Tables;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class Croponom : PreloadableSingleton<Croponom>, ISoundStarter {
        public FactsPage FactsPage;
        public Button GridButtonPrefab;

        [Header("Crops")]
        public Toggle CropsOpenButton;

        public GameObject CropsPage;
        public GameObject CropsGrid;
        public CropsTable CropsTablePrefab;

        [Header("Tools")]
        public Toggle ToolsOpenButton;

        public GameObject ToolsPage;
        public GameObject ToolsGrid;
        public ToolsTable ToolsTablePrefab;

        [Header("Weather")]
        public Toggle WeatherOpenButton;

        public GameObject WeatherPage;
        public GameObject WeatherGrid;
        public WeatherTable WeatherTablePrefab;

        private List<Button> _cropsButtons;
        private List<Button> _toolButtons;
        private List<Button> _weatherButtons;

        [SerializeField]
        private GameObject Panel;

        public Action OnClose;

        /**********/

        protected override void OnFirstInit() {
            GenerateAllButtons();
        }

        public void Open() {
            Panel.SetActive(true);
        }

        public void OpenCropsPage(bool isOpen) {
            CropsPage.SetActive(isOpen);
            ToolsPage.SetActive(!isOpen);
            WeatherPage.SetActive(!isOpen);
        }

        public void OpenToolsPage(bool isOpen) {
            CropsPage.SetActive(!isOpen);
            ToolsPage.SetActive(isOpen);
            WeatherPage.SetActive(!isOpen);
        }

        public void OpenWeathersPage(bool isOpen) {
            CropsPage.SetActive(!isOpen);
            ToolsPage.SetActive(!isOpen);
            WeatherPage.SetActive(isOpen);
        }

        public void OpenOnPage(string pageName) {
            ConfigWithCroponomPage pageConfig = CropsTablePrefab.Crops.FirstOrDefault(c => c.type.ToString() == pageName);
            if (pageConfig != null) {
                Open();
                OpenCropsPage(true);
                OpenPage(pageConfig);
            }

            pageConfig = ToolsTablePrefab.ToolsSO.FirstOrDefault(c => c.buff.ToString() == pageName);
            if (pageConfig != null) {
                Open();
                OpenToolsPage(true);
                OpenPage(pageConfig);
            }

            pageConfig = WeatherTablePrefab.WeathersSO.FirstOrDefault(c => c.type.ToString() == pageName);
            if (pageConfig != null) {
                Open();
                OpenWeathersPage(true);
                OpenPage(pageConfig);
            }
        }

        public void Close() {
            OnClose?.Invoke();
            Panel.SetActive(false);
        }

        private void SubscribeTabButtons() {
            /*  CropsOpenButton.onValueChanged.AddListener((_) => TryOpenPage(CropsTablePrefab.Crops[0],_));
              WeatherOpenButton.onValueChanged.AddListener((_) => TryOpenPage(WeatherTablePrefab.WeathersSO[0],_));
              ToolsOpenButton.onValueChanged.AddListener((_) => TryOpenPage(ToolsTablePrefab.ToolsSO[0],_));
         */
        }

        private void ReleaseAllButtons() {
            CropsOpenButton.onValueChanged.RemoveListener((_) => TryOpenPage(CropsTablePrefab.Crops[0], _));
            WeatherOpenButton.onValueChanged.RemoveListener((_) => TryOpenPage(WeatherTablePrefab.WeathersSO[0], _));
            ToolsOpenButton.onValueChanged.RemoveListener((_) => TryOpenPage(ToolsTablePrefab.ToolsSO[0], _));
            foreach (Button button in _cropsButtons) button.onClick.RemoveAllListeners();

            foreach (Button button in _weatherButtons) button.onClick.RemoveAllListeners();

            foreach (Button button in _toolButtons) button.onClick.RemoveAllListeners();
        }

        private void TryOpenPage(ConfigWithCroponomPage pageData, bool check) {
            if (check) {
                OpenPage(pageData);
            }
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

                weatherButton.GetComponent<Image>().sprite = weatherData.gridIcon;
                _weatherButtons.Add(weatherButton);
            }
        }

        private void GenerateCropsButtons() {
            _cropsButtons = new List<Button>();
            foreach (CropConfig cropData in CropsTablePrefab.Crops) {
                Button cropButton = Instantiate(GridButtonPrefab, CropsGrid.transform);
                cropButton.onClick.AddListener(() => FactsPage.UpdatePage(cropData));

                cropButton.GetComponent<Image>().sprite = cropData.gridIcon;
                _cropsButtons.Add(cropButton);
            }
        }

        private void GenerateToolsButtons() {
            _toolButtons = new List<Button>();
            foreach (ToolConfig toolData in ToolsTablePrefab.ToolsSO) {
                Button toolButton = Instantiate(GridButtonPrefab, ToolsGrid.transform);
                toolButton.onClick.AddListener(() => FactsPage.UpdatePage(toolData));

                toolButton.GetComponent<Image>().sprite = toolData.gridIcon;
                _toolButtons.Add(toolButton);
            }
        }

        public void PlaySound(int soundIndex) {
            Audio.Instance.PlaySound((Sounds)soundIndex);
        }
    }
}