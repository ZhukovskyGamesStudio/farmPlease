using System.Linq;
using Abstract;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

namespace UI {
    public class UIHud : Singleton<UIHud>, ISoundStarter {
        public Transform CenterFarmTransform;
        public BatteryView BatteryView;
        public ScreenEffect screenEffect;
        public FastPanelScript FastPanelScript;
        public TimePanel TimePanel;
        public Backpack Backpack;
        public ShopsPanel ShopsPanel;

        public GameObject CroponomButton;
        public AttentionView CroponomAttention, BackpackAttention;
        public OpenCroponomButtonView OpenCroponomButton;

        public GameObject HammerToolButton;
        public GameObject BuildingPanel;
        public GraphicRaycaster GraphicRaycaster;

        public GameObject SettingsButton;

        public ClockView ClockView;
       
        public SpotlightWithText SpotlightWithText;
        public KnowledgeCanSpeak KnowledgeCanSpeak;
        public Croponom Croponom;
        public ProfileView ProfileView;
        public CountersView CountersView;
        protected override bool IsDontDestroyOnLoad => false;

        public FarmerCommunityBadgeView FarmerCommunityBadgeView;
        public GameObject HomeFarmUi;
        public OtherFarmUI OtherFarmUI;

        public void ClosePanel() {
            if (Backpack.isOpen)
                Backpack.OpenClose();
        }

        public void SetBattery(int amount) {
            BatteryView.UpdateCharge(amount);
        }

        public void NoEnergy() {
            BatteryView.NoEnergy();
        }

        public void SetCounters() {
            CountersView.SetData(SaveLoadManager.CurrentSave);
            ProfileView.SetData(SaveLoadManager.CurrentSave);
        }

        public void ChangeInventoryHover(int index) {
            FastPanelScript.UpdateHover(index);
        }

        public void ChangeFastPanel(Crop crop, int amount) {
            FastPanelScript.UpdateSeedFastPanel(crop, amount);
        }

        public void OpenBuildingsShop() {
            ShopsPanel.BuildingShopButton.gameObject.SetActive(true);
        }

        public void CloseBuildingsShop() {
            if (!GameModeManager.Instance.IsBuildingsShopAlwaysOpen) {
                ShopsPanel.BuildingShopButton.gameObject.SetActive(false);
            }
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
            UIHud.Instance.Croponom.Open();
        }

        public void OpenSettings() {
            Settings.Instance.OpenSettings();
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
            TimePanel.gameObject.SetActive(!isActive);
            FastPanelScript.gameObject.SetActive(!isActive);
            BatteryView.gameObject.SetActive(!isActive);
        }

        public void UpdateLockedUI() {
            TimePanel.gameObject.SetActive(KnowledgeUtils.HasKnowledge(Knowledge.Weather));
            ShopsPanel.ToolShopButton.gameObject.SetActive(UnlockableUtils.HasUnlockable(Unlockable.ToolShop.ToString()));
            
            var buildindsUnlocked = UnlockableUtils.HasUnlockable(Unlockable.FoodMarket.ToString());
            var isTodayFoodmarket = TimeManager.Instance.IsTodayFoodMarket();
            ShopsPanel.BuildingShopButton.gameObject.SetActive(buildindsUnlocked && isTodayFoodmarket);
            
            FarmerCommunityBadgeView.gameObject.SetActive(UnlockableUtils.HasUnlockable(Unlockable.FarmerCommunity.ToString()));
            
            HammerToolButton.SetActive(InventoryManager.IsBuildingsBoughtD.Values.Any(v=>v));
        }
    }
}