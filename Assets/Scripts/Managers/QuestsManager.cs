using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Managers;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

public class QuestsManager : Singleton<QuestsManager> {
    [SerializeField]
    private List<QuestConfig> _mainQuests;

    [SerializeField]
    private List<GeneratableQuestConfig> _generatableQuests;

    private QuestsDialogData QuestsData => SaveLoadManager.CurrentSave.QuestsData;

    public TimeSpan TimeToQuestsUpdate => QuestsData.LastTimeQuestsUpdatedDateTime +
        TimeSpan.FromHours(ConfigsManager.Instance.CostsConfig.HoursQuestsChange) - DateTime.Now;

    private QuestsDialog _questsDialog;

    public bool IsDailyUnlocked => SaveLoadManager.CurrentSave.CurrentLevel >= ConfigsManager.Instance.CostsConfig.LevelToUnlockDaily-1;
    
    
    public void TryStartQuestsTimer() {
        QuestsUpdateTimer(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask QuestsUpdateTimer(CancellationToken cancellationToken) {
        var delay = TimeToQuestsUpdate;
        if (delay.TotalSeconds < 0) {
            GenerateSideQuests();
        } else {
            await UniTask.Delay(TimeToQuestsUpdate, cancellationToken: cancellationToken);
        }

        await QuestsUpdateTimer(cancellationToken);
    }

    public void GenerateNextMainQuest() {
        if (QuestsData.MainQuest == null) {
            QuestsData.MainQuest = new QuestData();
            QuestsData.MainQuest.Copy(_mainQuests[QuestsData.MainQuestProgressIndex].QuestData);
        }
    }

    public void ProgressMainQuestline() {
        QuestsData.MainQuestProgressIndex++;

        QuestsData.MainQuest = new QuestData();
        if (QuestsData.MainQuestProgressIndex >= _mainQuests.Count) {
            QuestsData.MainQuest.Copy(GenerateRandomizedQuest());
        } else {
            QuestsData.MainQuest.Copy(_mainQuests[QuestsData.MainQuestProgressIndex].QuestData);
        }

        if (_questsDialog != null) {
            _questsDialog.ShowMainQuestChange(QuestsData.MainQuest);
        }
    }

    private void GenerateSideQuests() {
        QuestsData.FirstQuest = GenerateRandomizedQuest();
        QuestsData.SecondQuest = GenerateRandomizedQuest();

        if (_questsDialog != null) {
            _questsDialog.ShowSideQuestChange(QuestsData.FirstQuest, QuestsData.SecondQuest);
        }

        QuestsData.LastTimeQuestsUpdated = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture);
        SaveLoadManager.SaveGame();
    }

    private QuestData GenerateRandomizedQuest() {
        var rnd = _generatableQuests[Random.Range(0, _generatableQuests.Count)];
    
        var data = new QuestData();
        data.Copy(rnd.QuestData);
        var rndMultiplier = Random.Range(rnd.MinMultiplier, rnd.MaxMultiplier + 1);
        data.ProgressNeeded *= rndMultiplier;
        data.XpReward *= rndMultiplier;
        return data;
    }

    public void OpenQuestsDialog() {
        var d = DialogsManager.Instance.ShowDialogWithData(typeof(QuestsDialog), new QuestsDialogData() {
            MainQuest = QuestsData.MainQuest,
            FirstQuest = QuestsData.FirstQuest,
            SecondQuest = QuestsData.SecondQuest
        });
        if (d != null) {
            _questsDialog = d as QuestsDialog;
        }
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

    public void ChangeQuestForAd(int questIndex) {
        if (questIndex == 0) {
            QuestsData.FirstQuest = GenerateRandomizedQuest();
        } else {
            QuestsData.SecondQuest = GenerateRandomizedQuest();
        }
        if (_questsDialog != null) {
            _questsDialog.ShowSideQuestChange(QuestsData.FirstQuest, QuestsData.SecondQuest);
        }
    }

    
}