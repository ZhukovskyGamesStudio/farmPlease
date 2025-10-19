using System;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

namespace UI {
    public class Croponom : Singleton<Croponom>, ISoundStarter {
        public FactsPage FactsPage;

        [SerializeField]
        private FactsPage _animationPage;

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

        [SerializeField]
        private Animation _pageAnimaion;

        [SerializeField]
        private AnimationClip _turnRight, _turnLeft;

        public Action OnClose;

        private int _curPageIndex;
        private ConfigWithCroponomPage _curPage;

        protected override bool IsDontDestroyOnLoad => false;

        protected override void OnFirstInit() {
            if (_cropsButtons == null) {
                GenerateAllButtons();
            }

            if (SaveLoadManager.CurrentSave.UnseenCroponomPages.Count > 0) {
                UIHud.Instance.CroponomAttention.ShowAttention();
            }
        }

        private void GenerateAllButtons() {
            //TODO optimize so generating only part of buttons
            //rest of buttons generate after
            //and remake via dialog
            _cropsButtons = GenerateButtons(CropsTable.Instance.Crops, CropsGrid.transform);
            _weatherButtons = GenerateButtons(WeatherTable.Instance.WeathersSO, WeatherGrid.transform);
            _toolButtons = GenerateButtons(ToolsTable.Instance.ToolsSO, ToolsGrid.transform);
            _buildingsButtons = GenerateButtons(BuildingsTable.Instance.Buildings, BuildingsGrid.transform);
            OpenPage(CropsTable.Instance.Crops.FirstOrDefault(c => c.type == Crop.Tomato));
        }

        private List<CroponomGridButtonView> GenerateButtons<TConfig>(IEnumerable<TConfig> configs, Transform parent)
            where TConfig : ConfigWithCroponomPage {
            List<CroponomGridButtonView> buttonList = new List<CroponomGridButtonView>();
            configs = configs.OrderBy(c => c.GetPageIndex());
            foreach (var config in configs) {
                CroponomGridButtonView button = Instantiate(GridButtonPrefab, parent);
                button.SetData(config, OnGridButtonClick);
                buttonList.Add(button);
            }

            return buttonList;
        }

        private void OnGridButtonClick(ConfigWithCroponomPage c) {
            if (c.GetPageIndex() == _curPageIndex) {
                return;
            }
            if (c.GetPageIndex() > _curPageIndex) {
                _animationPage.UpdatePage(_curPage);
                FactsPage.UpdatePage(c);

                _pageAnimaion.Play(_turnLeft.name);
            } else {
                FactsPage.UpdatePage(_curPage);
                _animationPage.UpdatePage(c);
                _pageAnimaion.Play(_turnRight.name);
            }

            _curPageIndex = c.GetPageIndex();
            _curPage = c;
        }

        private void Open() {
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
            WeatherOpenButton.gameObject.SetActive(KnowledgeUtils.HasKnowledge(Knowledge.Weather));
            BuildingsOpenButton.gameObject.SetActive(SaveLoadManager.CurrentSave.BuildingShopData.BuildingPriceIndex > 0);
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

            if (_cropsButtons == null) {
                GenerateAllButtons();
            }

            CroponomGridButtonView selected = null;
            if (pageConfig != null) {
                selected = _cropsButtons.First(b => b.GetUnlockable() == pageName);
                Open();
                OpenCropsPage(true);
                OpenPage(pageConfig);
                CropsOpenButton.SetIsOnWithoutNotify(true);
            }

            pageConfig = ToolsTable.Instance.ToolsSO.FirstOrDefault(c => c.buff.ToString() == pageName);
            if (pageConfig != null) {
                selected = _toolButtons.First(b => b.GetUnlockable() == pageName);

                Open();
                OpenToolsPage(true);
                OpenPage(pageConfig);
                ToolsOpenButton.SetIsOnWithoutNotify(true);
            }

            pageConfig = WeatherTable.Instance.WeathersSO.FirstOrDefault(c => c.type.ToString() == pageName);
            if (pageConfig != null) {
                selected = _weatherButtons.First(b => b.GetUnlockable() == pageName);
                Open();
                OpenWeathersPage(true);
                OpenPage(pageConfig);
                WeatherOpenButton.SetIsOnWithoutNotify(true);
            }

            pageConfig = BuildingsTable.Instance.Buildings.FirstOrDefault(c => c.type.ToString() == pageName);
            if (pageConfig != null) {
                selected = _buildingsButtons.First(b => b.GetUnlockable() == pageName);
                Open();
                OpenBuildingsPage(true);
                OpenPage(pageConfig);
                BuildingsOpenButton.SetIsOnWithoutNotify(true);
            }

            if (selected != null) {
                if (SaveLoadManager.CurrentSave.UnseenCroponomPages.Contains(pageName)) {
                    SaveLoadManager.CurrentSave.UnseenCroponomPages.Remove(pageName);
                }

                EventSystem.current.SetSelectedGameObject(selected.gameObject);
                selected.SetAttentionState(false);
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
            _curPageIndex = pageData.GetPageIndex();
            _curPage = pageData;
        }

        public void PlaySound(int soundIndex) {
            Audio.Instance.PlaySound((Sounds)soundIndex);
        }
    }
}