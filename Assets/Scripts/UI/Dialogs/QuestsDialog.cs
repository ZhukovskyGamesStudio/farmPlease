using System;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
using UnityEngine;

public class QuestsDialog : DialogWithData<QuestsDialogData> {
    [SerializeField]
    private QuestView _mainQuestView, _firstQuestView, _secondQuestView;

    [SerializeField]
    private MainQuestLockedView _mainQuestLockedView;

    [SerializeField]
    private GameObject _mainTab, _secondaryTab;

    [SerializeField]
    private TextMeshProUGUI _timerText;
    
    
    public override void SetData(QuestsDialogData data) {
        SetMainQuestData(data.MainQuest);
        _firstQuestView.SetData(data.FirstQuest);
        _secondQuestView.SetData(data.SecondQuest);
        QuestsUpdateTimer(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid QuestsUpdateTimer(CancellationToken cancellationToken) {
        while (true) {
            _timerText.text = $"Обновится через {TimeUtils.ToShortString(QuestsManager.Instance.TimeToQuestsUpdate)}";
            await UniTask.Delay(1, cancellationToken: cancellationToken);
        }
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
    public void ShowSideQuestChange(QuestData newQuest, QuestData newQuest1) {
        _firstQuestView.SetData(newQuest);
        _secondQuestView.SetData(newQuest1);
    }

    public void OpenMain(bool isOn) {
        _mainTab.gameObject.SetActive(isOn);
    }

    public void OpenOther(bool isOn) {
        _secondaryTab.gameObject.SetActive(isOn);
    }
}

[Serializable]
public class QuestsDialogData {
    public int MainQuestProgressIndex;
    public QuestData MainQuest;
    public QuestData FirstQuest, SecondQuest;
    public string LastTimeQuestsUpdated = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
    public DateTime LastTimeQuestsUpdatedDateTime => DateTime.Parse(LastTimeQuestsUpdated, CultureInfo.InvariantCulture);
    public bool IsUnseenUpdate;
}
