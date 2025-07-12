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
        public Transform CenterFarmTransform, QuestboardTransform;
        public BatteryView BatteryView;
        public ScreenEffect screenEffect;
        public FastPanelScript FastPanelScript;
        public TimePanel TimePanel;
        public Backpack Backpack;
        public ShopsPanel ShopsPanel;
        public Button QuestsInvisibleButton;

        public GameObject CroponomButton;
        public AttentionView CroponomAttention, BackpackAttention;
        public OpenCroponomButtonView OpenCroponomButton;

        public GameObject HammerToolButton;
        public BuildingStatePanel BuildingPanel;
        public GraphicRaycaster GraphicRaycaster;

        public Button SettingsButton;

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
        public OpenRealShopButtonView OpenRealShopButton;

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
            BuildingPanel.gameObject.SetActive(isActive);
            ClockView.gameObject.SetActive(!isActive);
            CountersView.gameObject.SetActive(!isActive);
            SettingsButton.gameObject.SetActive(!isActive);
            CroponomButton.gameObject.SetActive(!isActive);
            ShopsPanel.gameObject.SetActive(!isActive);
            Backpack.gameObject.SetActive(!isActive);
            TimePanel.gameObject.SetActive(!isActive);
            FastPanelScript.gameObject.SetActive(!isActive);
            BatteryView.gameObject.SetActive(!isActive);
            OpenRealShopButton.gameObject.SetActive(!isActive);
            ProfileView.gameObject.SetActive(!isActive);
            HammerToolButton.gameObject.SetActive(!isActive);
            QuestsInvisibleButton.gameObject.SetActive(!isActive);
        }

        public void UpdateLockedUI() {
            TimePanel.gameObject.SetActive(KnowledgeUtils.HasKnowledge(Knowledge.Weather));
            ShopsPanel.ToolShopButton.gameObject.SetActive(UnlockableUtils.HasUnlockable(Unlockable.ToolShop.ToString()));

            var buildindsUnlocked = UnlockableUtils.HasUnlockable(Unlockable.FoodMarket.ToString());
            var isTodayFoodmarket = TimeManager.Instance.IsTodayFoodMarket();
            ShopsPanel.BuildingShopButton.gameObject.SetActive(buildindsUnlocked && isTodayFoodmarket);

            FarmerCommunityBadgeView.gameObject.SetActive(FarmerCommunityManager.Instance.IsUnlocked &&
                                                          FarmerCommunityManager.Instance.IsNextFarmLoaded);

            HammerToolButton.SetActive(InventoryManager.IsBuildingsBoughtD.Values.Any(v => v));
        }

        public void OpenQuests() {
            QuestsManager.Instance.OpenQuestsDialog();
        }
    }
}