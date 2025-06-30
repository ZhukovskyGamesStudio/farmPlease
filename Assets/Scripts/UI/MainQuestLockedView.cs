using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainQuestLockedView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _headerText, _progressText;

    [SerializeField]
    private Slider _slider;

    public void SetData(int nextLevel, float percent) {
        _headerText.text = LocalizationUtils.L("main_quests_unlock_level").Replace("%N", nextLevel.ToString());
        _progressText.text = $"{LocalizationUtils.L("main_quests_progress")} {XpUtils.CurrentProgressAsText()}";
        _slider.value = percent;
    }
}