using System.Collections.Generic;
using UnityEngine;

public class WatchAdRewardView : MonoBehaviour {
    [SerializeField]
    private List<RewardItemView> _rewardItems;

    public virtual void SetData(WatchAdDialog.Data data) {
        if (_rewardItems is { Count: > 0 }) {
            RewardUtils.SetRewardsView(data.Reward, _rewardItems, null);
        }
    }
}