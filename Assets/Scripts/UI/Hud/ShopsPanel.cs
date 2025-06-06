using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopsPanel : MonoBehaviour {
        public Button ScalesButton;
        public ScalesDialog ScalesView;
        
        public Button SeedShopButton, ToolShopButton;
      
        public GameObject BuildingButton;
        public BuildingShopView BuildingShopView;
        public Button BuildingShopButton;
        
        public void OpenSeedshop() {
            DialogsManager.Instance.ShowDialogWithData(typeof(SeedShopDialog), SaveLoadManager.CurrentSave.SeedShopData);
        }
        
        public void OpenScales() {
            ScalesView.gameObject.SetActive(true);
        }
        public void OpenToolShop() {
            DialogsManager.Instance.ShowDialogWithData(typeof(ToolShopDialog), SaveLoadManager.CurrentSave.ToolShopData);
        }
        public void OpenBuildingsShop() {
            BuildingShopView.gameObject.SetActive(true);
        }
    }
}