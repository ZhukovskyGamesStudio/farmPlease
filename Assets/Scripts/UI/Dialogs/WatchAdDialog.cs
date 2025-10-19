using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dialogs;
using Tables;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class WatchAdDialog : DialogWithData<WatchAdDialog.Data> {
    public class Data {
        public Reward Reward;
        public Action OnClaim;
        public string Header;
        public string AdId;
        public WatchAdRewardView RewardViewPrefab;
    }

    [SerializeField]
    private Transform _rewardContainer;

    [SerializeField]
    private RewardItemView _rewardItemView;

    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _dialogShow, _dialogIdle, _watchAd;

    [SerializeField]
    private TextMeshProUGUI _header;

    [SerializeField]
    private Image _justImage;

    private bool _isWatchingAd;
    private Data _data;

    public override void SetData(Data data) {
        _data = data;

        _header.text = data.Header;

        if (data.RewardViewPrefab != null) {
            var view = Instantiate(data.RewardViewPrefab, _rewardContainer);
            view.SetData(data);
        }

        /*
        if (_data.IsJustImage) {
            _justImage.gameObject.SetActive(true);
            _rewardItemView.gameObject.SetActive(false);
            _justImage.sprite = _data.JustSprite;
            _justImage.SetNativeSize();
        } else {
            RewardUtils.SetRewardsView(data.Reward, new List<RewardItemView>() {
                _rewardItemView
            }, null);
        }*/
    }

    public override async UniTask Show(Action onClose, Action<bool> onHideUI) {
        UIHud.Instance.ProfileView.Hide();
        await base.Show(onClose, onHideUI);
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
        ZhukovskyAdsManager.Instance.AdsProvider.ShowRewardedAd(_data.AdId, () => {
            _isWatchingAd = false;
            _data.OnClaim?.Invoke();
            Close();
        }, () => {
            _isWatchingAd = false;
            Close();
        });

        _animation.Play(_watchAd.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
    }
}