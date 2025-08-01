﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

public class QuestsManager : Singleton<QuestsManager> {
    [SerializeField]
    private QuestsConfig _questsConfig;

    private QuestsDialogData QuestsData => SaveLoadManager.CurrentSave.QuestsData;

    public TimeSpan TimeToQuestsUpdate => QuestsData.LastTimeQuestsUpdatedDateTime +
        TimeSpan.FromHours(ConfigsManager.Instance.CostsConfig.HoursQuestsChange) - DateTime.Now;

    private QuestsDialog _questsDialog;

    public bool IsDailyUnlocked => LevelsUtils.IsDailyUnlocked;
    
    
    public void TryStartQuestsTimer() {
        if (!IsDailyUnlocked) {
            return;
        }
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
            QuestsData.MainQuest.Copy(_questsConfig.MainQuestline[QuestsData.MainQuestProgressIndex].QuestData);
        }
    }

    public void ProgressMainQuestline() {
        QuestsData.MainQuestProgressIndex++;

        QuestsData.MainQuest = new QuestData();
        if (QuestsData.MainQuestProgressIndex >= _questsConfig.MainQuestline.Count) {
            QuestsData.MainQuest.Copy(GenerateRandomizedQuest());
        } else {
            QuestsData.MainQuest.Copy(_questsConfig.MainQuestline[QuestsData.MainQuestProgressIndex].QuestData);
        }

        TryChangeSpecial(QuestsData.MainQuest);
        InventoryManager.Instance.RetriggerCollectionQuests();
        if (_questsDialog != null) {
            _questsDialog.ShowMainQuestChange(QuestsData.MainQuest);
        }
        SaveLoadManager.CurrentSave.QuestsData.IsUnseenUpdate = true;
        if (KnowledgeUtils.HasKnowledge(Knowledge.Training)) {
            QuestsUtils.ChangeTileView(SaveLoadManager.CurrentSave.QuestsData);
        }
      
    }

    public void GenerateSideQuests() {
        QuestsData.FirstQuest = GenerateRandomizedQuest();
        QuestsData.SecondQuest = GenerateRandomizedQuest();

        if (_questsDialog != null) {
            _questsDialog.ShowSideQuestChange(QuestsData.FirstQuest, QuestsData.SecondQuest);
        }

        TryChangeSpecial(QuestsData.FirstQuest);
        TryChangeSpecial(QuestsData.SecondQuest);
        InventoryManager.Instance.RetriggerCollectionQuests();
        QuestsData.LastTimeQuestsUpdated = DateTime.Now.Date.ToString(CultureInfo.InvariantCulture);
        SaveLoadManager.CurrentSave.QuestsData.IsUnseenUpdate = true;
        if (KnowledgeUtils.HasKnowledge(Knowledge.Training)) {
            QuestsUtils.ChangeTileView(SaveLoadManager.CurrentSave.QuestsData);
        }
        SaveLoadManager.SaveGame();
    }

    private QuestData GenerateRandomizedQuest() {
        var possible = _questsConfig.GeneratableQuestConfigs.Where(q =>
            q.MinLevel <= SaveLoadManager.CurrentSave.CurrentLevel && UnlockableUtils.HasUnlockable(q.Unlockable)).ToList();
        var rnd = possible[Random.Range(0, possible.Count)];
    
        var data = new QuestData();
        data.Copy(rnd.QuestData);
        var rndMultiplier = Random.Range(rnd.MinMultiplier, rnd.MaxMultiplier + 1);
        data.ProgressNeeded *= rndMultiplier;
        data.XpReward *= rndMultiplier;
        return data;
    }

    public void OpenQuestsDialog() {
        SmartTilemap.Instance.BrobotAnimTilemap.ShowFlyAnimation();
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

        if (quest.QuestType == QuestTypes.Special) {
            TryChangeSpecial(quest);
        } else {
            if (isSet) {
                quest.Progress = change;
            } else {
                quest.Progress += change;
            }
        }
     

        if (quest.Progress <= 0) {
            quest.Progress = 0;
        }

        if (quest.Progress >= quest.ProgressNeeded) {
            quest.IsCompleted = true;
        }
        SaveLoadManager.SaveGame();
    }

    public static void TryChangeSpecial( QuestData quest) {
        if (quest.TriggerName.Contains(SpecialTargetTypes.DigAllField.ToString())) {
            var total = TileUtils.CountAvailableTiles();
            quest.Progress = total - TileUtils.CountTilesOfType(TileType.Sand);
            quest.ProgressNeeded = total;
        }
        SaveLoadManager.SaveGame();
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
        SaveLoadManager.SaveGame();
    }

    public void MarkQuestClaimed(int questIndex) {
        if (questIndex == 0) {
            QuestsData.FirstQuest.IsClaimed = true;
        } else {
            QuestsData.SecondQuest.IsClaimed = true;
        }
        if (_questsDialog != null) {
            _questsDialog.ShowSideQuestChange(QuestsData.FirstQuest, QuestsData.SecondQuest);
        }
        SaveLoadManager.SaveGame();
    }

    
}