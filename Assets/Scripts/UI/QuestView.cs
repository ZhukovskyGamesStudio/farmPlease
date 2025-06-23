using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _text, _xpRewardText,_progressText;

    [SerializeField]
    private Slider _questProgressSlider;
    
    [SerializeField]
    private RewardItemView _rewardItemView;

    private QuestData _questData;

    public void SetData(QuestData data) {
        _questData = data;
        _text.text = data.QuestText;
        _xpRewardText.text = data.XpReward.ToString();
        _progressText.text = QuestsUtils.GetQuestProgress(data);
        _questProgressSlider.value = QuestsUtils.GetQuestProgressPercent(data);
    }

    public void ClaimClick() {
        QuestsUtils.ClaimQuest(_questData);
    }
}