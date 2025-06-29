using UI;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratableQuestConfig", menuName = "Scriptable Objects/GeneratableQuestConfig", order = 0)]
public class GeneratableQuestConfig : QuestConfig {
    [Header("Generated")]
    public int MinMultiplier;
    public int MaxMultiplier;
}