using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WatchAdRewardView : MonoBehaviour {
    [SerializeField]
    private List<RewardItemView> _rewardItems;

    [SerializeField]
    private TextMeshProUGUI _durationText;

    public virtual void SetData(WatchAdDialog.Data data) {
        if (_rewardItems is { Count: > 0 }) {
            RewardUtils.SetRewardsView(data.Reward, _rewardItems, null);
        }

        if (_durationText != null) {
            SetDurationText(data);
        }
    }

    private void SetDurationText(WatchAdDialog.Data data) {
        int seconds = data.Reward.Items.First().Amount;
        if (seconds <= 90) {
            _durationText.text = $"{seconds} {ZG_Localization.LocalizationManager.Instance.GetText("seconds")}";
        } else {
            _durationText.text = $"{seconds / 60} {ZG_Localization.LocalizationManager.Instance.GetText("minutes")}";
        }
    }
}