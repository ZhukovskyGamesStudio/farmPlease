using System.Collections.Generic;
using Managers;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;

public class QuestsManager : Singleton<QuestsManager> {
    [SerializeField]
    private List<QuestConfig> _mainQuests;

    [SerializeField]
    private List<QuestConfig> _generatableQuests;

    private QuestsData QuestsData => SaveLoadManager.CurrentSave.QuestsData;

    public void GenerateMainQuest() {
        if (QuestsData.MainQuest == null) {
            QuestsData.MainQuest = new QuestData();
            QuestsData.MainQuest.Copy(_mainQuests[QuestsData.MainQuestProgressIndex].QuestData);
        }
    }
    
    public void OpenQuestsDialog() {
        DialogsManager.Instance.ShowDialogWithData(typeof(QuestsDialog), new QuestsDialogData() {
            MainQuest = QuestsData.MainQuest,
            FirstQuest = QuestsData.FirstQuest,
            SecondQuest = QuestsData.SecondQuest
        });
    }

    public static void TriggerQuest(string triggerName, int change, bool isSet = false) {
        TryChangeQuestProgress(triggerName, change, isSet, Instance.QuestsData.MainQuest);
        TryChangeQuestProgress(triggerName, change, isSet, Instance.QuestsData.FirstQuest);
        TryChangeQuestProgress(triggerName, change, isSet, Instance.QuestsData.SecondQuest);
    }

    private static void TryChangeQuestProgress(string triggerName, int change, bool isSet, QuestData quest) {
        if (quest == null) {
            return;
        }

        if (quest.TriggerName != triggerName) {
            return;
        }

        if (isSet) {
            quest.Progress = change;
        } else {
            quest.Progress += change;
        }

        if (quest.Progress <= 0) {
            quest.Progress = 0;
        }

        if (quest.Progress >= quest.ProgressNeeded) {
            quest.IsCompleted = true;
        }
    }
}