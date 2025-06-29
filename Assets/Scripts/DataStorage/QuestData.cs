using System;
using UnityEngine;

[Serializable]
public class QuestData {
    public bool IsMain;
    public int MinLevelToUnlock;

    [field: SerializeField]
    public QuestTypes QuestType { get; private set; }

    [field: SerializeField]
    [QuestTargetDropdown]
    public string TargetType;

    public string TriggerName => QuestType + TargetType;
   
    
    [HideInInspector]
    public int Progress;

    [HideInInspector]
    public bool IsCompleted;
    [HideInInspector]
    public bool IsClaimed;
    [HideInInspector]
    public Reward Reward;
    
    [Header("Texts")]
    public string QuestName;
    public string QuestText;
    
    [Header("Values")]
    public int ProgressNeeded;
    public int XpReward;

    public void Copy(QuestData other) {
        if (other == null) return;
        IsMain = other.IsMain;
        MinLevelToUnlock = other.MinLevelToUnlock;
        ProgressNeeded = other.ProgressNeeded;
        QuestName = other.QuestName;
        QuestText = other.QuestText;
        XpReward = other.XpReward;
        Progress = other.Progress;
        IsCompleted = other.IsCompleted;
        QuestType = other.QuestType;
        TargetType = other.TargetType;

        // Глубокое копирование Reward
        if (other.Reward != null) {
            if (other.Reward is RewardWithUnlockable rwu) {
                var newReward = new RewardWithUnlockable();
                newReward.Unlockable = rwu.Unlockable;
                newReward.Items = new System.Collections.Generic.List<RewardItem>();
                foreach (var item in rwu.Items)
                    newReward.Items.Add(new RewardItem { Type = item.Type, Amount = item.Amount });
                Reward = newReward;
            } else {
                var newReward = new Reward();
                newReward.Items = new System.Collections.Generic.List<RewardItem>();
                foreach (var item in other.Reward.Items)
                    newReward.Items.Add(new RewardItem { Type = item.Type, Amount = item.Amount });
                Reward = newReward;
            }
        } else {
            Reward = null;
        }
    }
}