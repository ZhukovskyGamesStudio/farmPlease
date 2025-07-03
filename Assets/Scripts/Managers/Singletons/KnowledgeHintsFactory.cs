using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;

public class KnowledgeHintsFactory : MonoBehaviour {
    public static KnowledgeHintsFactory Instance { get; private set; }

    [SerializeField]
    private SpotlightAnimConfig _noEnergyHint, _toolShopHint, _foodMarketHint, _farmerCommunityHint;

    [SerializeField]
    private SpotlightAnimConfig _calendarHint, _weatherHint, _happeningHint;

    private void Awake() {
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
                if (!KnowledgeUtils.HasKnowledge(Knowledge.ToolShop)) {
                    ShowToolShopHint();
                }

                break;
            case nameof(Unlockable.FoodMarket):
                if (!KnowledgeUtils.HasKnowledge(Knowledge.FoodMarket)) {
                    ShowFoodMarketHint();
                }

                break;
            case nameof(Unlockable.FarmerCommunity):
                if (!KnowledgeUtils.HasKnowledge(Knowledge.FarmerCommunity)) {
                    ShowFarmerCommunityHint();
                }

                break;
        }
    }

    private void ShowToolShopHint() {
        SmartTilemap.Instance.BrobotAnimTilemap.ShowFlyAnimation();
        UIHud.Instance.ShopsPanel.ToolShopButton.gameObject.SetActive(true);
        UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.ShopsPanel.ToolShopButton, _toolShopHint, delegate {
            KnowledgeUtils.AddKnowledge(Knowledge.ToolShop); 
            SmartTilemap.Instance.BrobotAnimTilemap.ShowLandAnimation();
        }, true);
    }

    private void ShowFoodMarketHint() {
        SmartTilemap.Instance.BrobotAnimTilemap.ShowFlyAnimation();
        UIHud.Instance.ShopsPanel.BuildingShopButton.gameObject.SetActive(true);
        UIHud.Instance.ShopsPanel.BuildingShopButton.interactable = true;
        UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.ShopsPanel.BuildingShopButton.transform, _foodMarketHint, delegate {
            KnowledgeUtils.AddKnowledge(Knowledge.FoodMarket); 
            SmartTilemap.Instance.BrobotAnimTilemap.ShowLandAnimation();
        }, true, true);
    
    }

    public void TryShowNoEnergyHint() {
        if (KnowledgeUtils.HasKnowledge(Knowledge.NoEnergy)) {
            return;
        }
        SmartTilemap.Instance.BrobotAnimTilemap.ShowFlyAnimation();
        UnlockableUtils.Unlock(ToolBuff.WeekBattery);
        UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.ClockView.transform, _noEnergyHint, GiveBatteryReward, true, true);
    }

    private static void GiveBatteryReward() {
        SmartTilemap.Instance.BrobotAnimTilemap.ShowLandAnimation();
        DialogsManager.Instance.ShowDialogWithData(typeof(RewardDialog), new RewardDialogData() {
            Reward = new Reward() {
                Items = new List<RewardItem>() {
                    new RewardItem() {
                        Type = ToolBuff.WeekBattery.ToString(),
                        Amount = 1
                    }
                }
            },
            OnClaim = delegate {
                KnowledgeUtils.AddKnowledge(Knowledge.NoEnergy);
                UIHud.Instance.BackpackAttention.ShowAttention();
                
            }
        });
    }

    private void ShowFarmerCommunityHint() {
        if (KnowledgeUtils.HasKnowledge(Knowledge.FarmerCommunity)) {
            return;
        }
        SmartTilemap.Instance.BrobotAnimTilemap.ShowFlyAnimation();
        UIHud.Instance.FarmerCommunityBadgeView.gameObject.SetActive(true);
        UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.FarmerCommunityBadgeView.transform, _farmerCommunityHint, delegate {
            KnowledgeUtils.AddKnowledge(Knowledge.FarmerCommunity); 
            SmartTilemap.Instance.BrobotAnimTilemap.ShowLandAnimation();
        }, true, true);
    }

    public void TryShowCalendarHint() {
        SmartTilemap.Instance.BrobotAnimTilemap.ShowFlyAnimation();
        UIHud.Instance.TimePanel.gameObject.SetActive(true);
        UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.TimePanel.CalendarButton, _calendarHint, ShowWeatherHint);
    }

    private void ShowWeatherHint() {
        SmartTilemap.Instance.BrobotAnimTilemap.ShowLandAnimation();
        BigCalendarDialog bigCalendarDialog = FindAnyObjectByType<BigCalendarDialog>();
        UIHud.Instance.SpotlightWithText.ShowSpotlight(bigCalendarDialog.transform, _weatherHint,
            delegate { KnowledgeUtils.AddKnowledge(Knowledge.Weather); }, true, true);
    }

    public void TryShowHappeningHint() {
        SmartTilemap.Instance.BrobotAnimTilemap.ShowFlyAnimation();
        UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.Instance.TimePanel.HappeningButton, _happeningHint, delegate {
            KnowledgeUtils.AddKnowledge(Knowledge.LilCalendar);
            SmartTilemap.Instance.BrobotAnimTilemap.ShowLandAnimation();
        }, true);
    }
}