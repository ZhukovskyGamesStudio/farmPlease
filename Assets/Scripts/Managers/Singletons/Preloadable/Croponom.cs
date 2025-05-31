using System;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

namespace UI {
    public class Croponom : Singleton<Croponom>, ISoundStarter {
        public FactsPage FactsPage;
        public CroponomGridButtonView GridButtonPrefab;

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

        private List<CroponomGridButtonView> _cropsButtons;
        private List<CroponomGridButtonView> _toolButtons;
        private List<CroponomGridButtonView> _weatherButtons;

        [SerializeField]
        private GameObject Panel;

        public Action OnClose;

        protected override bool IsDontDestroyOnLoad => false;

        protected override void OnFirstInit() {
            GenerateAllButtons();
        }
        
        private void GenerateAllButtons() {
            _cropsButtons = GenerateButtons(CropsTablePrefab.Crops, CropsGrid.transform);
            _weatherButtons = GenerateButtons(WeatherTablePrefab.WeathersSO, WeatherGrid.transform);
            _toolButtons = GenerateButtons(ToolsTablePrefab.ToolsSO, ToolsGrid.transform);
            OpenPage(CropsTablePrefab.Crops.FirstOrDefault(c => c.type == Crop.Tomato));
        }

        private List<CroponomGridButtonView> GenerateButtons<TConfig>(IEnumerable<TConfig> configs, Transform parent)
            where TConfig : ConfigWithCroponomPage {
            List<CroponomGridButtonView> buttonList = new List<CroponomGridButtonView>();
            foreach (var config in configs) {
                CroponomGridButtonView button = Instantiate(GridButtonPrefab, parent);
                button.SetData(config, FactsPage.UpdatePage);
                buttonList.Add(button);
            }

            return buttonList;
        }

        public void Open() {
            Panel.SetActive(true);
            UIHud.Instance.ProfileView.Hide();
            foreach (CroponomGridButtonView button in _cropsButtons) {
                button.SetLockState(UnlockableUtils.HasUnlockable(button.GetUnlockable()));
            }

            foreach (CroponomGridButtonView button in _toolButtons) {
                button.SetLockState(UnlockableUtils.HasUnlockable(button.GetUnlockable()));
            }

            foreach (CroponomGridButtonView button in _weatherButtons) {
                button.SetLockState(UnlockableUtils.HasUnlockable(button.GetUnlockable()));
            }
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
            UIHud.Instance.ProfileView.Show();
        }

        private void OpenPage(ConfigWithCroponomPage pageData) {
            FactsPage.UpdatePage(pageData);
        }

       

        public void PlaySound(int soundIndex) {
            Audio.Instance.PlaySound((Sounds)soundIndex);
        }
    }
}