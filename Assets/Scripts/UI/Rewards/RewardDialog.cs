using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RewardDialog : DialogBase {
    private Reward _reward;

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

    public void Show(Reward reward) {
        _reward = reward;
        List<RewardItemView> combinedRewardViews = new List<RewardItemView>();
        combinedRewardViews.Add(_mainReward);
        combinedRewardViews.AddRange(_additionalRewards);
       

        int rewardsAmount = reward.Items.Count + (reward is RewardWithUnlockable ? 1 : 0);
        for (int i = 0; i < rewardsAmount; i++) {
            if (i == 0 && reward is RewardWithUnlockable rewardWithUnlockable) {
                combinedRewardViews[i].SetData(RewardUtils.GetRewardIcon(rewardWithUnlockable.Unlockable), rewardWithUnlockable.Unlockable);
                continue;
            }

            var item = reward.Items[i+ (reward is RewardWithUnlockable ? -1 : 0)];
            Sprite icon = item.Type == RewardUtils.COINS_KEY ? _coinRewardIcon : RewardUtils.GetRewardIcon(item.Type);
            combinedRewardViews[i].SetData(icon, item.Amount);
            if (item.Type != RewardUtils.COINS_KEY) {
                _clicksNeeded += item.Amount;
            } else {
                _clicksNeeded++;
            }
        }

        _clicksNeeded++;
        _clicksMade = 0;
        gameObject.SetActive(true);
        _chestAnimation.Play(_chestAppear.name);
        _chestAnimation.PlayQueued(_chestIdle.name);
    }

    public void ClickChest() {
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

        int rewardsAmount = _reward.Items.Count;
        if (_reward is RewardWithUnlockable) {
            rewardsAmount++;
        }

        _rewardsAnimation.Play(_rewardsAnimationClips[rewardsAmount - 1].name);
    }

    public void Claim() {
        //TODO show items flying animation
        RewardUtils.ClaimReward(_reward);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}