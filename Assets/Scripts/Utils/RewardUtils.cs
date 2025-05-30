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
    
    public static void ClaimReward(Reward reward) {
        if (reward is RewardWithUnlockable rewardWithUnlockable) {
            UnlockableUtils.Unlock(rewardWithUnlockable.Unlockable);
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
}