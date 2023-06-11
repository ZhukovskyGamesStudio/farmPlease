using System;
using System.Collections.Generic;

namespace DefaultNamespace.Tables {
    public class KnowledgeManager {
        private static List<Knowledge> SavedKnowledge => SaveLoadManager.CurrentSave?.KnowledgeList;

        public static bool HasKnowledge(Knowledge kn) => SavedKnowledge?.Contains(kn) ?? false;

        public static void AddKnowledge(Knowledge kn) {
            SavedKnowledge?.Add(kn);
            SaveLoadManager.Instance.SaveGame();
        }
    }

    [Serializable]
    public enum Knowledge {
        Training = 0
    }
}