﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UI;
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

    [SerializeField]
    private TextMeshProUGUI _headerText;
    private bool _isShowing;
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
            _headerText.text = "Вы разблокировали";
        } else {
            _headerText.text = "Вы получили";
        }

        _clicksNeeded++;
        _clicksMade = 0;
        _isShowing = true;
    }

    public override async UniTask Show(Action onClose) {
        UIHud.Instance.ProfileView.Hide();
        _chestAnimation.Play(_chestAppear.name);

        await base.Show(onClose);
        await UniTask.WaitWhile(() => _chestAnimation.isPlaying);
        _chestAnimation.PlayQueued(_chestIdle.name);
        _isShowing = false;
    }

    public void ClickChest() {
        if (_isShowing) {
            return;
        }
        if (_clicksMade >= _clicksNeeded) {
            return;
        }

        _clicksMade++;

        if (_clicksMade >= _clicksNeeded) {
            OpenChest();
        } else {
            _chestAnimation.Stop();
            _chestAnimation.Play(_chestClick.name);
            _chestAnimation.PlayQueued(_chestIdle.name);
        }
    }

    private async void OpenChest() {
        _chestAnimation.Stop();
        _chestAnimation.Play(_chestClick.name);
        _chestAnimation.PlayQueued(_chestOpen.name);
        await UniTask.WaitWhile(() => _chestAnimation.isPlaying);

        int rewardsAmount = _data.Reward.Items.Count;
        if (_data.Reward is RewardWithUnlockable) {
            rewardsAmount++;
        }

        _rewardsAnimation.Play(_rewardsAnimationClips[rewardsAmount - 1].name);
    }

    public void Claim() {
        UIHud.Instance.ProfileView.Show();
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