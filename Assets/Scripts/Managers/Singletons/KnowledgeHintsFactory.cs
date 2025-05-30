using Managers;
using ScriptableObjects;
using UI;
using UnityEngine;

public class KnowledgeHintsFactory: MonoBehaviour
{
 public   static KnowledgeHintsFactory Instance { get; private set; }

 [SerializeField]
 private SpotlightAnimConfig _toolShopHint, _foodMarketHint;


    private void Awake()
    {
        Instance = this;
    }
    
    
    public void CheckAllUnshownHints() {
        foreach (string unlockable in SaveLoadManager.CurrentSave.Unlocked) {
            TryShowHintByUnlockable(unlockable);
        }
    }

    public void TryShowHintByUnlockable(string unlockable) {
        switch (unlockable) {
            case nameof(Unlockable.ToolShop):
                if( !KnowledgeUtils.HasKnowledge(Knowledge.ToolShop)) {
                    ShowToolShopHint();
                }
               
                break;
            case nameof(Unlockable.FoodMarket):
                if (!KnowledgeUtils.HasKnowledge(Knowledge.FoodMarket)) {
                    ShowFoodMarketHint();
                }

                break;
        }
    }
    
    public void ShowToolShopHint() {
       
        UIHud.Instance.ShopsPanel.ToolShopButton.gameObject.SetActive(true);
            UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.ToolShopButton, _toolShopHint,
                delegate { KnowledgeUtils.AddKnowledge(Knowledge.ToolShop); }, true);
        
    }
    
    public void ShowFoodMarketHint() {
        UIHud.Instance.ShopsPanel.BuildingShopButton.gameObject.SetActive(true);
        UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.ShopsPanel.BuildingShopButton.transform, _foodMarketHint,
            delegate { KnowledgeUtils.AddKnowledge(Knowledge.FoodMarket); }, true);
        
    }
 
}
