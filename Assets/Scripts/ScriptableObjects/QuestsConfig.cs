using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestsConfig", menuName = "Scriptable Objects/QuestsConfig", order = 0)]
public class QuestsConfig : ScriptableObject {
    public List<QuestConfig> MainQuestline;
    public List<GeneratableQuestConfig> GeneratableQuestConfigs;
}