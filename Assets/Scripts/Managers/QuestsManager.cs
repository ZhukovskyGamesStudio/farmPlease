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

    public void OpenQuestsDialog() {
        DialogsManager.Instance.ShowDialogWithData(typeof(QuestsDialog), new QuestsDialogData() {
            MainQuest = _mainQuests[QuestsData.MainQuestProgressIndex].QuestData,
            FirstQuest = QuestsData.FirstQuest,
            SecondQuest = QuestsData.SecondQuest
        });
    }
}