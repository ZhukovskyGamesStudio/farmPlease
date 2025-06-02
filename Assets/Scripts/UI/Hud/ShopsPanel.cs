using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopsPanel : MonoBehaviour {
        public Button ScalesButton;
        public ScalesPanelView ScalesView;
        
        public Button SeedShopButton, ToolShopButton;
      
        public GameObject BuildingButton;
        public SeedShopView seedShopView;
        public BuildingShopView BuildingShopView;
        public Button BuildingShopButton;

        public void OpenToolShop() {
            DialogsManager.Instance.ShowDialogWithData(typeof(ToolShopDialog), SaveLoadManager.CurrentSave.ToolShopData);
        }
        
    }
}