using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _text, _xpRewardText, _progressText;

    [SerializeField]
    private Slider _questProgressSlider;

    [SerializeField]
    private RewardItemView _rewardItemView;

    private QuestData _questData;

    public void SetData(QuestData data) {
        if (data == null) {
            return;
        }

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

    public void ClaimClick() {
        if (_questData == null) {
            return;
        }

        if (_questData.IsCompleted) {
            QuestsUtils.ClaimQuest(_questData);
        } else { }
    }

    public void ShowChangeToNextQuest(QuestData data) {
       
    }
}