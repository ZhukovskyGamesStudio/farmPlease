using TMPro;
using UnityEngine;

public class QuestView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _text, _progressText;

    [SerializeField]
    private RewardItemView _rewardItemView;

    private QuestData _questData;

    public void SetData(QuestData data) {
        _questData = data;
        _text.text = data.QuestText;
        _progressText.text = QuestsUtils.GetQuestProgress(data);
    }

    public void ClaimClick() {
        QuestsUtils.ClaimQuest(_questData);
    }
}