using System;
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
}

[Serializable]
public class QuestData {
    public bool IsMain;
    public string QuestText;
    public Reward Reward;
}