﻿using System;
using System.Collections.Generic;
using Managers;
using Tables;
using UI;

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
        if (unlockable == Unlockable.FoodMarket.ToString()) {
            TimeManager.AddMissingFoodMarkets();
        } else if (unlockable == Unlockable.ToolShop.ToString()) {
            SaveLoadManager.CurrentSave.UnseenCroponomPages.Add(ToolBuff.Unlimitedwatercan.ToString());
        } else if (unlockable == Unlockable.Field1.ToString()) {
            TileUtils.UnlockTiles(TileUtils.GenerateCircleTiles(SmartTilemap.STARTING_CIRCLE_RADIUS + 1));
            GameSceneEntryPoint.UpdateDecorUpgradeState();
        } else if (unlockable == Unlockable.Field2.ToString()) {
            TileUtils.UnlockTiles(TileUtils.GenerateCircleTiles(SmartTilemap.STARTING_CIRCLE_RADIUS + 2));
            GameSceneEntryPoint.UpdateDecorUpgradeState();
        }

        AddAndSave(unlockable);
    }

    private static void AddAndSave(string unlockable) {
        if (Unlocked.Contains(unlockable)) {
            return;
        }

        if (!NotInCroponom.Contains(unlockable)) {
            SaveLoadManager.CurrentSave.UnseenCroponomPages.Add(unlockable);
        }
      
        Unlocked?.Add(unlockable);
        if (SaveLoadManager.Instance != null) {
            SaveLoadManager.SaveGame();
        }
        
        if (LoadingManager.IsGameLoaded) {
            UIHud.Instance.CroponomAttention.ShowAttention();
            UIHud.Instance.OpenCroponomButton.UpdateTags();
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

    public static void TryRemoveSeenPage(string page) {
        if( !SaveLoadManager.CurrentSave.UnseenCroponomPages.Contains(page)) {
            return;
        }
        SaveLoadManager.CurrentSave.UnseenCroponomPages.Remove(page);
        SaveLoadManager.SaveGame();
    }

    public static List<string> NotInCroponom = new List<string> {
        Unlockable.ToolShop.ToString(),
        Unlockable.FarmerCommunity.ToString(),
        Unlockable.None.ToString(),
        Unlockable.Field1.ToString(),
        Unlockable.Field2.ToString()
    };
    
}

[Serializable]
public enum Unlockable {
    None = -1,
    ToolShop = 0,
    FoodMarket = 1,
    FarmerCommunity = 2,
    Field1 = 3,
    Field2 = 4
}