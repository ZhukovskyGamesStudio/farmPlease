using DefaultNamespace.Abstract;
using DefaultNamespace.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHud : Singleton<UIHud>, ISoundStarter {
    public BatteryView BatteryView;
    public CoinsScript CoinsScript;
    public FastPanelScript FastPanelScript;
    public TimePanel TimePanel;
    public Backpack Backpack;
    public ShopsPanel ShopsPanel;
    public HelloPanelView HelloPanel;

    public GameObject CroponomButton;
    public GameObject BuildingPanel;
    public GraphicRaycaster GraphicRaycaster;

    public ClockView ClockView;
    public SpotlightWithText SpotlightWithText;
    public KnowledgeCanSpeak KnowledgeCanSpeak;

    public void ClosePanel() {
        if (Settings.Instance.SettingsPanel.gameObject.activeSelf)
            Settings.Instance.SettingsPanel.gameObject.SetActive(false);
        else if (TimePanel.isOpen)
            TimePanel.CalendarPanelOpenClose();
        else if (ShopsPanel.ToolShopPanel.gameObject.activeSelf)
            ShopsPanel.ToolShopPanel.gameObject.SetActive(false);
        else if (ShopsPanel.seedShopScript.gameObject.activeSelf)
            ShopsPanel.seedShopScript.gameObject.SetActive(false);
        else if (Backpack.isOpen)
            Backpack.OpenClose();
    }

    public void SetBattery(int amount) {
        BatteryView.UpdateCharge(amount);
    }

    public void NoEnergy() {
        BatteryView.NoEnergy();
    }

    public void ChangeCoins(int amount) {
        CoinsScript.UpdateCoins(amount);
    }

    public void ChangeInventoryHover(int index) {
        FastPanelScript.UpdateHover(index);
    }

    public void ChangeFastPanel(CropsType crop, int amount) {
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
        Gps.Instance.ShowLeaderBoard();
    }
}