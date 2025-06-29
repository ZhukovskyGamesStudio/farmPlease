using UnityEngine;

namespace UI {
    [CreateAssetMenu(fileName = "QuestConfig", menuName = "Scriptable Objects/QuestConfig", order = 0)]
    public class QuestConfig : ScriptableObject {
        public QuestData QuestData;
    }
}