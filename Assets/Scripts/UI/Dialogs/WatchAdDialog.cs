using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Tables;
using UI;
using UnityEngine;

public class WatchAdDialog : DialogWithData<Reward> {
    [SerializeField]
    private RewardItemView _rewardItemView;

    [SerializeField]
    private Animation _animation, _tvAnimation;

    [SerializeField]
    private AnimationClip _dialogShow, _watchAd, _tvIdle;

    private bool _isWatchingAd;

    public override void SetData(Reward data) {
        RewardUtils.SetRewardsView(data, new List<RewardItemView>() {
            _rewardItemView
        }, null);
    }

    public override void Show(Action onClose) {
        base.Show(onClose);
        _tvAnimation.Play(_tvIdle.name);
        _animation.Play(_dialogShow.name);
        UIHud.Instance.ProfileView.Hide();
    }

    public override void Close() {
        UIHud.Instance.ProfileView.Show();
        base.Close();
    }

    public void WatchRewardedAdButton() {
        if (_isWatchingAd) {
            return;
        }

        _isWatchingAd = true;
        WatchRewardedAd().Forget();
    }

    private async UniTask WatchRewardedAd() {
        DialogsManager.Instance.ShowDialogWithData(typeof(RewardDialog), new RewardDialogData() {
            Reward = new Reward() {
                Items = new List<RewardItem>() {
                    new RewardItem() {
                        Type = ToolBuff.WeekBattery.ToString(),
                        Amount = 1
                    }
                }
            },
            OnClaim = () => { UIHud.Instance.BackpackAttention.ShowAttention(); }
        });
        _animation.Play(_watchAd.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
        Close();
    }
}