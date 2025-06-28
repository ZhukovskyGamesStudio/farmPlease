using System;
using System.Globalization;
using UnityEngine;

public class QuestsDialog : DialogWithData<QuestsDialogData> {
    [SerializeField]
    private QuestView _mainQuestView, _firstQuestView, _secondQuestView;

    public override void SetData(QuestsDialogData data) {
        _mainQuestView.SetData(data.MainQuest);
        _firstQuestView.SetData(data.FirstQuest);
        _secondQuestView.SetData(data.SecondQuest);
    }
}

[Serializable]
public class QuestsDialogData {
    public QuestData MainQuest;
    public QuestData FirstQuest, SecondQuest;
    public string LastTimeQuestsUpdated = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
    public DateTime LastTimeQuestsUpdatedDateTime => DateTime.Parse(LastTimeQuestsUpdated, CultureInfo.InvariantCulture);
    public bool IsUnseenUpdate;
}

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
    public int ProgressNeeded;

    public string QuestName;
    public string QuestText;
    public Reward Reward;
    public int XpReward;
    public int Progress;
    
    public bool IsCompleted;

    public void Copy(QuestData other) {
        if (other == null) return;
        IsMain = other.IsMain;
        MinLevelToUnlock = other.MinLevelToUnlock;
        ProgressNeeded = other.ProgressNeeded;
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