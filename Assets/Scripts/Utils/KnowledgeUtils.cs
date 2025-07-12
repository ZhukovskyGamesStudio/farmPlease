using System;
using System.Collections.Generic;
using Managers;

public static class KnowledgeUtils {
    private static List<Knowledge> SavedKnowledge => SaveLoadManager.CurrentSave?.KnowledgeList;

    public static bool HasKnowledge(Knowledge kn) => SavedKnowledge?.Contains(kn) ?? false;

    public static void AddKnowledge(Knowledge kn) {
        SavedKnowledge?.Add(kn);
        if (SaveLoadManager.Instance != null) {
            SaveLoadManager.SaveGame();
        }
    }
}

[Serializable]
public enum Knowledge {
    Training = 0,
    Weather = 1,
    LilCalendar = 2,
    ToolShop = 3,
    FoodMarket = 4,
    NoEnergy = 5,
    FarmerCommunity = 6,
    GotBattery = 7
}