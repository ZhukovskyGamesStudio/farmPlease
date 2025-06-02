using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RewardDialog : DialogWithData<RewardDialogData> {
    private RewardDialogData _data;

    [SerializeField]
    private RewardItemView _mainReward;

    [SerializeField]
    private List<RewardItemView> _additionalRewards;

    [SerializeField]
    private Sprite _coinRewardIcon;

    [SerializeField]
    private Animation _chestAnimation, _rewardsAnimation;

    [SerializeField]
    private AnimationClip _chestAppear, _chestIdle, _chestClick, _chestOpen;

    [SerializeField]
    private List<AnimationClip> _rewardsAnimationClips = new List<AnimationClip>();

    private int _clicksNeeded, _clicksMade;

    public override void SetData(RewardDialogData data) {
        _data = data;
        List<RewardItemView> combinedRewardViews = new List<RewardItemView>();
        combinedRewardViews.Add(_mainReward);
        combinedRewardViews.AddRange(_additionalRewards);
        var items = _data.Reward.Items;
        RewardUtils.SetRewardsView(_data.Reward, combinedRewardViews, _coinRewardIcon);
        for (int i = 0; i < items.Count; i++) {
            var item = items[i];

            if (item.Type != RewardUtils.COINS_KEY) {
                _clicksNeeded += item.Amount;
            } else {
                _clicksNeeded++;
            }
        }

        if (data.Reward is RewardWithUnlockable) {
            _clicksNeeded++;
        }

        _clicksNeeded++;
        _clicksMade = 0;
    }

    public override void Show(Action onClose) {
        base.Show(onClose);
        _chestAnimation.Play(_chestAppear.name);
        _chestAnimation.PlayQueued(_chestIdle.name);
    }

    public void ClickChest() {
        if (_clicksMade >= _clicksNeeded) {
            return;
        }

        _clicksMade++;
        _chestAnimation.Play(_chestClick.name);
        _chestAnimation.PlayQueued(_chestIdle.name);
        if (_clicksMade >= _clicksNeeded) {
            OpenChest();
        }
    }

    private async void OpenChest() {
        _chestAnimation.Play(_chestOpen.name);
        await UniTask.WaitWhile(() => _chestAnimation.isPlaying);

        int rewardsAmount = _data.Reward.Items.Count;
        if (_data.Reward is RewardWithUnlockable) {
            rewardsAmount++;
        }

        _rewardsAnimation.Play(_rewardsAnimationClips[rewardsAmount - 1].name);
    }

    public void Claim() {
        //TODO show items flying animation
        RewardUtils.ClaimReward(_data.Reward);
        _data.OnClaim?.Invoke();
        Close();
    }
}
[Serializable]
public class RewardDialogData {
    public Reward Reward;
    public Action OnClaim;
}