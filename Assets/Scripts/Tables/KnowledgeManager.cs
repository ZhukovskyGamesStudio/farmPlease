using System;
using System.Collections.Generic;
using Managers;

namespace Tables {
    public class KnowledgeManager {
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
        FoodMarket = 3,
    }
}