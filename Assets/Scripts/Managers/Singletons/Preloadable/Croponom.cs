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

        [Header("Tools")]
        public Toggle ToolsOpenButton;

        public GameObject ToolsPage;
        public GameObject ToolsGrid;

        [Header("Weather")]
        public Toggle WeatherOpenButton;

        public GameObject WeatherPage;
        public GameObject WeatherGrid;

        [Header("Buildings")]
        public Toggle BuildingsOpenButton;

        public GameObject BuildingsPage;
        public GameObject BuildingsGrid;

        private List<CroponomGridButtonView> _cropsButtons;
        private List<CroponomGridButtonView> _toolButtons;
        private List<CroponomGridButtonView> _weatherButtons;
        private List<CroponomGridButtonView> _buildingsButtons;

        [SerializeField]
        private GameObject Panel;

        public Action OnClose;

        protected override bool IsDontDestroyOnLoad => false;

        protected override void OnFirstInit() {
            GenerateAllButtons();

            if (SaveLoadManager.CurrentSave.UnseenCroponomPages.Count > 0) {
                UIHud.Instance.CroponomAttention.ShowAttention();
            }
        }

        private void GenerateAllButtons() {
            _cropsButtons = GenerateButtons(CropsTable.Instance.Crops, CropsGrid.transform);
            _weatherButtons = GenerateButtons(WeatherTable.Instance.WeathersSO, WeatherGrid.transform);
            _toolButtons = GenerateButtons(ToolsTable.Instance.ToolsSO, ToolsGrid.transform);
            _buildingsButtons = GenerateButtons(BuildingsTable.Instance.Buildings, BuildingsGrid.transform);
            OpenPage(CropsTable.Instance.Crops.FirstOrDefault(c => c.type == Crop.Tomato));
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

            List<CroponomGridButtonView> buttons = new();
            buttons.AddRange(_cropsButtons);
            buttons.AddRange(_toolButtons);
            buttons.AddRange(_weatherButtons);
            buttons.AddRange(_buildingsButtons);

            foreach (CroponomGridButtonView button in buttons) {
                button.SetLockState(UnlockableUtils.HasUnlockable(button.GetUnlockable()));
                button.SetAttentionState(SaveLoadManager.CurrentSave.UnseenCroponomPages.Contains(button.GetUnlockable()));
            }

            ToolsOpenButton.gameObject.SetActive(UnlockableUtils.HasUnlockable(ToolBuff.WeekBattery));
            WeatherOpenButton.gameObject.SetActive(KnowledgeUtils.HasKnowledge(Knowledge.LilCalendar));
            BuildingsOpenButton.gameObject.SetActive(SaveLoadManager.CurrentSave.BuildingShopData.BuildingPriceIndex > 0);

            if (SaveLoadManager.CurrentSave.UnseenCroponomPages.Count > 0) {
                var nextPage = SaveLoadManager.CurrentSave.UnseenCroponomPages.FirstOrDefault();
                if (_cropsButtons.Any(b => b.GetUnlockable() == nextPage)) {
                    OpenCropsPage(true);
                } else if (_toolButtons.Any(b => b.GetUnlockable() == SaveLoadManager.CurrentSave.UnseenCroponomPages.FirstOrDefault())) {
                    OpenToolsPage(true);
                } else if (_weatherButtons.Any(b => b.GetUnlockable() == SaveLoadManager.CurrentSave.UnseenCroponomPages.FirstOrDefault())) {
                    OpenWeathersPage(true);
                }

                if (_buildingsButtons.Any(b => b.GetUnlockable() == SaveLoadManager.CurrentSave.UnseenCroponomPages.FirstOrDefault())) {
                    OpenBuildingsPage(true);
                }
            } else {
                UIHud.Instance.CroponomAttention.Hide();
            }
        }

        public void OpenCropsPage(bool isOpen) {
            CropsPage.SetActive(isOpen);
            ToolsPage.SetActive(!isOpen);
            WeatherPage.SetActive(!isOpen);
            BuildingsPage.SetActive(!isOpen);
        }

        public void OpenToolsPage(bool isOpen) {
            CropsPage.SetActive(!isOpen);
            ToolsPage.SetActive(isOpen);
            WeatherPage.SetActive(!isOpen);
            BuildingsPage.SetActive(!isOpen);
        }

        public void OpenWeathersPage(bool isOpen) {
            CropsPage.SetActive(!isOpen);
            ToolsPage.SetActive(!isOpen);
            WeatherPage.SetActive(isOpen);
            BuildingsPage.SetActive(!isOpen);
        }

        public void OpenBuildingsPage(bool isOpen) {
            CropsPage.SetActive(!isOpen);
            ToolsPage.SetActive(!isOpen);
            WeatherPage.SetActive(!isOpen);
            BuildingsPage.SetActive(isOpen);
        }

        public void OpenOnPage(string pageName) {
            ConfigWithCroponomPage pageConfig = CropsTable.Instance.Crops.FirstOrDefault(c => c.type.ToString() == pageName);
            if (pageConfig != null) {
                Open();
                OpenCropsPage(true);
                OpenPage(pageConfig);
            }

            pageConfig = ToolsTable.Instance.ToolsSO.FirstOrDefault(c => c.buff.ToString() == pageName);
            if (pageConfig != null) {
                Open();
                OpenToolsPage(true);
                OpenPage(pageConfig);
            }

            pageConfig = WeatherTable.Instance.WeathersSO.FirstOrDefault(c => c.type.ToString() == pageName);
            if (pageConfig != null) {
                Open();
                OpenWeathersPage(true);
                OpenPage(pageConfig);
            }

            pageConfig = BuildingsTable.Instance.Buildings.FirstOrDefault(c => c.type.ToString() == pageName);
            if (pageConfig != null) {
                Open();
                OpenBuildingsPage(true);
                OpenPage(pageConfig);
            }
        }

        public void Close() {
            OnClose?.Invoke();
            Panel.SetActive(false);
            UIHud.Instance.ProfileView.Show();

            if (SaveLoadManager.CurrentSave.UnseenCroponomPages.Count == 0) {
                UIHud.Instance.CroponomAttention.Hide();
            }
        }

        private void OpenPage(ConfigWithCroponomPage pageData) {
            FactsPage.UpdatePage(pageData);
        }

        public void PlaySound(int soundIndex) {
            Audio.Instance.PlaySound((Sounds)soundIndex);
        }
    }
}