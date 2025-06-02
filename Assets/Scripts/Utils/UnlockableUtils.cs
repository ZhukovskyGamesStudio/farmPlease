using System;
using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;

public static class UnlockableUtils {
    private static List<string> Unlocked => SaveLoadManager.CurrentSave?.Unlocked;

    public static bool HasUnlockable(Crop unlockable) => HasUnlockable(unlockable.ToString());
    public static bool HasUnlockable(ToolBuff unlockable) => HasUnlockable(unlockable.ToString());
    public static bool HasUnlockable(HappeningType unlockable) => HasUnlockable(unlockable.ToString());
    public static bool HasUnlockable(BuildingType unlockable) => HasUnlockable(unlockable.ToString());
    public static bool HasUnlockable(string unlockable) => Unlocked?.Contains(unlockable) ?? false;

    public static void Unlock(Crop unlockable) {
        AddAndSave(unlockable.ToString());
    }
    
    public static void Unlock(BuildingType unlockable) {
        AddAndSave(unlockable.ToString());
    }

    public static void Unlock(ToolBuff unlockable) {
        AddAndSave(unlockable.ToString());
    }

    public static void Unlock(HappeningType unlockable) {
        AddAndSave(unlockable.ToString());
    }
    
    public static void Unlock(string unlockable) {
        AddAndSave(unlockable);
    }

    private static void AddAndSave(string unlockable) {
        if (Unlocked.Contains(unlockable)) {
            return;
        }

        UIHud.Instance.CroponomAttention.ShowAttention();
        Unlocked?.Add(unlockable);
        if (SaveLoadManager.Instance != null) {
            SaveLoadManager.SaveGame();
        }
    }

    public static List<string> GetInitialUnlockables() {
        var initialUnlockables = new List<string> {
            // Add initial unlockables here
            ToolBuff.Unlimitedwatercan.ToString(),
            Crop.Tomato.ToString(),
            HappeningType.Rain.ToString(),
            HappeningType.Unknown.ToString(),
            HappeningType.NormalSunnyDay.ToString()
        };

        return initialUnlockables;
    }

    
}

[Serializable]
public enum Unlockable {
    None = -1,
    ToolShop = 0,
    FoodMarket = 1,
}