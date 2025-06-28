using System;
using System.Globalization;
using Managers;
using UnityEngine;

public class QuestsDialog : DialogWithData<QuestsDialogData> {
    [SerializeField]
    private QuestView _mainQuestView, _firstQuestView, _secondQuestView;

    [SerializeField]
    private MainQuestLockedView _mainQuestLockedView;

    public override void SetData(QuestsDialogData data) {
        SetMainQuestData(data.MainQuest);
        _firstQuestView.SetData(data.FirstQuest);
        _secondQuestView.SetData(data.SecondQuest);
    }

    private void SetMainQuestData(QuestData data) {
        if (SaveLoadManager.CurrentSave.CurrentLevel < data.MinLevelToUnlock - 1) {
            _mainQuestLockedView.gameObject.SetActive(true);
            _mainQuestView.gameObject.SetActive(false);
            _mainQuestLockedView.SetData(data.MinLevelToUnlock, XpUtils.CurrentLevelProgress());
        } else {
            _mainQuestLockedView.gameObject.SetActive(false);
            _mainQuestView.gameObject.SetActive(true);
            _mainQuestView.SetData(data);
        }
    }

    public void ShowMainQuestChange(QuestData newQuest) {
        _mainQuestView.ShowChangeToNextQuest(newQuest);
        SetMainQuestData(newQuest);
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