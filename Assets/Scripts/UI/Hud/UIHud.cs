using Abstract;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

namespace UI {
    public class UIHud : Singleton<UIHud>, ISoundStarter {
        public BatteryView BatteryView;
        public ScreenEffect screenEffect;
        public FastPanelScript FastPanelScript;
        public TimePanelView TimePanelView;
        public Backpack Backpack;
        public ShopsPanel ShopsPanel;
        public HelloPanelView HelloPanel;

        public GameObject CroponomButton;
        public GameObject BuildingPanel;
        public GraphicRaycaster GraphicRaycaster;

        public GameObject SettingsButton;

        public ClockView ClockView;
        public CountersView CountersView;
        public SpotlightWithText SpotlightWithText;
        public KnowledgeCanSpeak KnowledgeCanSpeak;
        public ProfileDialog ProfileDialog;
        public Croponom Croponom;
        protected override bool IsDontDestroyOnLoad => false;

        public void ClosePanel() {
            if (Settings.Instance.SettingsPanel.gameObject.activeSelf)
                Settings.Instance.SettingsPanel.gameObject.SetActive(false);
            else if (TimePanelView.isOpen)
                TimePanelView.CalendarPanelOpenClose();
            else if (ShopsPanel.toolShopView.gameObject.activeSelf)
                ShopsPanel.toolShopView.gameObject.SetActive(false);
            else if (ShopsPanel.seedShopView.gameObject.activeSelf)
                ShopsPanel.seedShopView.gameObject.SetActive(false);
            else if (Backpack.isOpen)
                Backpack.OpenClose();
        }

        public void SetBattery(int amount) {
            BatteryView.UpdateCharge(amount);
        }

        public void NoEnergy() {
            BatteryView.NoEnergy();
        }

        public void SetCounters() {
            CountersView.SetCounters();
        }

        public void ChangeInventoryHover(int index) {
            FastPanelScript.UpdateHover(index);
        }

        public void ChangeFastPanel(Crop crop, int amount) {
            FastPanelScript.UpdateSeedFastPanel(crop, amount);
        }

        public void OpenBuildingsShop() {
            ShopsPanel.BuildingShopButton.interactable = true;
        }

        public void CloseBuildingsShop() {
            if (!GameModeManager.Instance.IsBuildingsShopAlwaysOpen)
                ShopsPanel.BuildingShopButton.interactable = false;
        }

        public void OpenMarketPlace() {
            ShopsPanel.ToolShopButton.GetComponent<Button>().interactable = true;
            ShopsPanel.SeedShopButton.GetComponent<Button>().interactable = false;
        }

        public void CloseMarketPlace() {
            ShopsPanel.ToolShopButton.GetComponent<Button>().interactable = false;
            ShopsPanel.SeedShopButton.GetComponent<Button>().interactable = true;
        }

        public void OpenCroponom() {
            Croponom.Instance.Open();
        }

        public void OpenSettings() {
            Settings.Instance.SettingsPanel.gameObject.SetActive(true);
        }

        public void LoadLevel(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

        public void GlobalRecordsButton() {
            GpsManager.Instance.ShowLeaderBoard();
        }

        public void SetBuildingPanelState(bool isActive) {
            BuildingPanel.SetActive(isActive);
            ClockView.gameObject.SetActive(!isActive);
            CountersView.gameObject.SetActive(!isActive);
            SettingsButton.gameObject.SetActive(!isActive);
            CroponomButton.gameObject.SetActive(!isActive);
            ShopsPanel.gameObject.SetActive(!isActive);
            Backpack.gameObject.SetActive(!isActive);
            TimePanelView.gameObject.SetActive(!isActive);
            FastPanelScript.gameObject.SetActive(!isActive);
            BatteryView.gameObject.SetActive(!isActive);
        }
    }
}