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
    private GameObject _collectState, _sendState, _completedState;

    [SerializeField]
    private Button _sendButton;

    private QuestData _questData;

    public void SetData(QuestData data) {
        if (data == null) {
            return;
        }

        UpdateState(data);

        _sendStateProgress.text = data.QuestText.Replace("%N", QuestsUtils.GetQuestProgress(data));
        _sendButton.interactable = QuestsUtils.GetQuestReadyForSend(data) >= data.ProgressNeeded;
        
        
        _questData = data;
        _text.text = data.QuestName + " -";
        _xpRewardText.text = data.XpReward.ToString();
        if (data.IsCompleted) {
            _progressText.text = "Нажми чтобы собрать награду";
            _questProgressSlider.value = 1;
        } else {
            _progressText.text = data.QuestText.Replace("%N", QuestsUtils.GetQuestProgress(data));
            _questProgressSlider.value = QuestsUtils.GetQuestProgressPercent(data);
        }
    }

    private void UpdateState(QuestData data) {
        _collectState.SetActive(!data.IsCompleted && data.QuestType == QuestTypes.Collect);
        _sendState.SetActive(!data.IsCompleted && data.QuestType == QuestTypes.Send);
        _completedState.SetActive(data.IsCompleted);
    }

    public void ClaimClick() {
        if (_questData == null) {
            return;
        }

        if ( _questData.IsCompleted) {
            QuestsUtils.ClaimQuest(_questData);
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

    public void ShowChangeToNextQuest(QuestData data) {
       
    }
}