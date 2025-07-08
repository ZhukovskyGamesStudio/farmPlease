using System;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Localization;
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
    private TextMeshProUGUI _timerText, _adsTimerText;

    [SerializeField]
    private TextMeshProUGUI _dailyQuestsText, _dailyQuestsLockedText;

    [SerializeField]
    private GameObject _dailyLocked;

    [SerializeField]
    private GameObject _dailyNormal, _dailyChangeForAds;

    private int _selectedQuestForChange;

    public override void SetData(QuestsDialogData data) {
        SetMainQuestData(data.MainQuest);
        _firstQuestView.SetData(data.FirstQuest);
        _secondQuestView.SetData(data.SecondQuest);
        QuestsUpdateTimer(this.GetCancellationTokenOnDestroy()).Forget();

        SetDailyLockedState(!QuestsManager.Instance.IsDailyUnlocked, ConfigsManager.Instance.CostsConfig.LevelToUnlockDaily);
    }

    private void SetDailyLockedState(bool isLocked, int lvlToUnlock) {
        _dailyLocked.gameObject.SetActive(isLocked);
        _dailyQuestsText.gameObject.SetActive(!isLocked);
        _dailyQuestsLockedText.text = $"{lvlToUnlock} {LocalizationUtils.L("quests_level")}";
    }

    private async UniTaskVoid QuestsUpdateTimer(CancellationToken cancellationToken) {
        while (true) {
            _timerText.text = $"{LocalizationUtils.L("quests_refresh")} {TimeUtils.ToShortString(QuestsManager.Instance.TimeToQuestsUpdate)}";
            _adsTimerText.text =
                $"{LocalizationUtils.L("quests_refresh_add_prefix")} {TimeUtils.ToShortString(QuestsManager.Instance.TimeToQuestsUpdate)}\n {LocalizationUtils.L("quests_refresh_add_suffix")}";

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
        _secondaryTab.gameObject.SetActive(!isOn);
    }

    public void OpenOther(bool isOn) {
        if (!QuestsManager.Instance.IsDailyUnlocked) {
            return;
        }

        _secondaryTab.gameObject.SetActive(isOn);
        _mainTab.gameObject.SetActive(!isOn);
    }

    public void OpenChangeForAds() {
        _dailyNormal.gameObject.SetActive(false);
        _dailyChangeForAds.gameObject.SetActive(true);
    }

    public void CloseChangeForAds() {
        _dailyNormal.gameObject.SetActive(true);
        _dailyChangeForAds.gameObject.SetActive(false);
    }

    public void SetQuestForChangeAfterAd1(bool isOn) {
        if (!isOn) {
            return;
        }

        _selectedQuestForChange = 0;
    }

    public void SetQuestForChangeAfterAd2(bool isOn) {
        if (!isOn) {
            return;
        }

        _selectedQuestForChange = 1;
    }

    public void ConfirmShowAd()
    {
        ShowAdAsync();
    }

    private Animation _watchadforquestanimation;
    private async void ShowAdAsync()
    {
        _watchadforquestanimation.Play("WatchAdForQuest");
            
       await UniTask.WaitWhile(() => _watchadforquestanimation.isPlaying);
        CloseChangeForAds();
        QuestsManager.Instance.ChangeQuestForAd(_selectedQuestForChange);
    }

    protected override UniTask Close() {
        SmartTilemap.Instance.BrobotAnimTilemap.ShowLandAnimation();
        return base.Close();
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