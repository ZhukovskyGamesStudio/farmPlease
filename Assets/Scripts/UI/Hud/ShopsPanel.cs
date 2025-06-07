using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopsPanel : MonoBehaviour {
        public Button ScalesButton;
        
        public Button SeedShopButton, ToolShopButton;
      
        public GameObject BuildingButton;
        public BuildingShopView BuildingShopView;
        public Button BuildingShopButton;
        
        public void OpenSeedshop() {
            DialogsManager.Instance.ShowDialogWithData(typeof(SeedShopDialog), SaveLoadManager.CurrentSave.SeedShopData);
        }
        
        public void OpenScales() {
            DialogsManager.Instance.ShowDialogWithData(typeof(ScalesDialog), 0);
        }
        public void OpenToolShop() {
            DialogsManager.Instance.ShowDialogWithData(typeof(ToolShopDialog), SaveLoadManager.CurrentSave.ToolShopData);
        }
        public void OpenBuildingsShop() {
            BuildingShopView.gameObject.SetActive(true);
        }
    }
}