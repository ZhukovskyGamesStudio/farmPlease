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
    private Animation _animation;

    [SerializeField]
    private AnimationClip _dialogShow,_dialogIdle, _watchAd;

    private bool _isWatchingAd;

    public override void SetData(Reward data) {
        RewardUtils.SetRewardsView(data, new List<RewardItemView>() {
            _rewardItemView
        }, null);
    }

    public override async UniTask Show(Action onClose) {
        UIHud.Instance.ProfileView.Hide();
        await base.Show(onClose);
        _animation.PlayQueued(_dialogIdle.name);
    }

    protected override async UniTask Close() {
        if (_isWatchingAd) {
            return;
        }
        UIHud.Instance.ProfileView.Show();
        await base.Close();
    }

       public void WatchRewardedAdButton() {
          if (_isWatchingAd) {
              return;
          }

          _isWatchingAd = true;
          WatchRewardedAd().Forget();
      }

     private async UniTask WatchRewardedAd() {
        
         ZhukovskyAdsManager.Instance.AdsProvider.ShowRewardedAd(AdsIds.RewardedBattery,() => {
             _isWatchingAd = false;
             GiveBatteryReward();
             Close();
         }, () => {
             _isWatchingAd = false;
             Close();
         });
         
         _animation.Play(_watchAd.name);
         await UniTask.WaitWhile(() => _animation.isPlaying);
      }

      private static void GiveBatteryReward() {
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
      }
}