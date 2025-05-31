using System;
using System.Collections.Generic;
using Managers;
using Tables;
using UnityEngine;

public static class RewardUtils {
    
    public static string COINS_KEY = CommonReward.Coins.ToString();
    
    public static Sprite GetRewardIcon(string reward) {
        if (Enum.TryParse(reward, out Crop crop)) {
            return CropsTable.CropByType(crop).gridIcon;
        }

        if (Enum.TryParse(reward, out ToolBuff tool)) {
            return ToolsTable.ToolByType(tool).gridIcon;
        }

        if (Enum.TryParse(reward, out HappeningType weather)) {
            return WeatherTable.WeatherByType(weather).gridIcon;
        }

        if (Enum.TryParse(reward, out Unlockable type)) {
            return ConfigsManager.Instance.UnclockableIcons.Find(icon => icon.Unlockable == reward)?.Icon;
        }

        throw new KeyNotFoundException();
    }
    
    public static ItemColorType GetRewardColorType(string reward) {
        if (Enum.TryParse(reward, out Crop crop)) {
            return ItemColorType.Seed;
        }

        if (Enum.TryParse(reward, out ToolBuff tool)) {
            if (tool == ToolBuff.WeekBattery) {
                return ItemColorType.Energy;
            }
            return ItemColorType.Tool;
        }

        return ItemColorType.None;
    }
    
    public static void ClaimReward(Reward reward) {
        if (reward is RewardWithUnlockable rewardWithUnlockable) {
            UnlockableUtils.Unlock(rewardWithUnlockable.Unlockable);
            KnowledgeHintsFactory.Instance.TryShowHintByUnlockable(rewardWithUnlockable.Unlockable);
        }

        foreach (var item in reward.Items) {
            var key = item.Type;
            if (key == COINS_KEY) {
                InventoryManager.Instance.AddCoins(item.Amount);
            }

            if (Enum.TryParse(key, out Crop crop)) {
                InventoryManager.Instance.AddSeed(crop,item.Amount);
            }

            if (Enum.TryParse(key, out ToolBuff tool)) {
                InventoryManager.Instance.AddTool(tool, item.Amount);
            }
        }
    }
    
    public static void SetRewardsView(Reward reward, List<RewardItemView> views, Sprite coinRewardIcon) {
        int rewardsAmount = reward.Items.Count + (reward is RewardWithUnlockable ? 1 : 0);
        foreach (RewardItemView itemView in views) {
            itemView.gameObject.SetActive(false);
        }
        for (int i = 0; i < rewardsAmount; i++) {
           
            
            if (i == 0 && reward is RewardWithUnlockable rewardWithUnlockable) {
                ItemColorType colorType = GetRewardColorType(rewardWithUnlockable.Unlockable);
                
                views[i].SetData(GetRewardIcon(rewardWithUnlockable.Unlockable), rewardWithUnlockable.Unlockable,colorType);
                continue;
            }
        
            RewardItem item = reward.Items[i + (reward is RewardWithUnlockable ? -1 : 0)];
            Sprite icon = item.Type == COINS_KEY ? coinRewardIcon : GetRewardIcon(item.Type);
            ItemColorType colorType2 = GetRewardColorType(item.Type);
            views[i].SetData(icon, item.Amount,colorType2);
        }
    }
}