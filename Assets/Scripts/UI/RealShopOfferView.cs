using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RealShopOfferView : MonoBehaviour {
    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _openAnimation, _openInstantClip;

    [SerializeField]
    private Button _buyButton;

    [SerializeField]
    private TextMeshProUGUI _timeText;
    [SerializeField]
    private TextMeshProUGUI _priceText;

    public Action _onBuy;
    private bool _isInteractable;
    private bool _isOpening;

    private void Start() {
        if (!_isInteractable) {
            _animation.Play(_openInstantClip.name);
        }
    }

    public void SetData(bool isInteractable, Action onBuy) {
        _onBuy = onBuy;
        _buyButton.interactable = isInteractable;
        _isInteractable = isInteractable;
    }

    public void SetPrice(string localizedPrice) {
        _priceText.text = localizedPrice;
    }

    public async UniTask StartTimer(TimeSpan timeLeft) {
        var token = this.GetCancellationTokenOnDestroy();
        while (timeLeft.TotalSeconds > 0) {
            if (!_isOpening) {
                _timeText.text = TimeUtils.ToShortString(timeLeft);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
            timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(1));
        }

        _timeText.text = "0s";
    }

    public void BuyClicked() {
        if (!_isInteractable) {
            return;
        }

        _onBuy?.Invoke();
    }

    public async UniTask ShowOpenAnimation() {
        _isOpening = true;
        _isInteractable = false;
        _animation.Play(_openAnimation.name);
        await UniTask.WaitWhile(() => _animation.isPlaying);
        _isOpening = false;
    }
}