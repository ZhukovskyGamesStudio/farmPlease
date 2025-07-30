using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _text, _xpRewardText, _progressText, _sendStateProgress;

    [SerializeField]
    private Slider _questProgressSlider;

    [SerializeField]
    private RewardItemView _rewardItemView;

    [SerializeField]
    private GameObject _collectState, _sendState, _completedState, _claimedState;

    [SerializeField]
    private Button _sendButton;

    private QuestData _questData;
    [SerializeField]
    private Color _dark, _bright;

    [SerializeField]
    private int _questIndex;
    
    public void SetData(QuestData data) {
        if (data == null) {
            gameObject.SetActive(false);
            return;
        }

        if (_claimedState) {
            _claimedState.SetActive(data.IsClaimed);
        }

        if (data.IsClaimed) {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        UpdateState(data);
        _questData = data;

        _text.text = LocalizationUtils.L(data.QuestNameLoc) + " -";
        _xpRewardText.text = data.XpReward.ToString();

        if (data.IsCompleted) {
            return;
        }

        _sendStateProgress.text = LocalizationUtils.L(data.QuestDescriptionLoc).Replace("%N", QuestsUtils.GetQuestSendProgress(data));
        _sendButton.interactable = QuestsUtils.GetQuestReadyForSend(data) >= data.ProgressNeeded;
        _sendStateProgress.color = _sendButton.interactable ? _bright : _dark;
        
       
        _progressText.text = LocalizationUtils.L(data.QuestDescriptionLoc).Replace("%N", QuestsUtils.GetQuestProgress(data));
        _questProgressSlider.value = QuestsUtils.GetQuestProgressPercent(data);
    }

    private void UpdateState(QuestData data) {
        _collectState.SetActive(!data.IsCompleted && (data.QuestType == QuestTypes.Collect || data.QuestType == QuestTypes.Special));
        _sendState.SetActive(!data.IsCompleted && data.QuestType == QuestTypes.Send);
        _completedState.SetActive(data.IsCompleted);
    }

    public void ClaimClick() {
        if (_questData == null) {
            return;
        }

        if (_questData.IsCompleted) {
            QuestsUtils.ClaimQuest(_questData);
            QuestsManager.Instance.MarkQuestClaimed(_questIndex);
        }
    }

    public void SendClick() {
        if (_questData == null) {
            return;
        }

        if (QuestsUtils.IsReadyForSend(_questData)) {
            QuestsUtils.SendQuest(_questData);
            UpdateState(_questData);
        }
    }

    public void ShowChangeToNextQuest(QuestData data) { }
}