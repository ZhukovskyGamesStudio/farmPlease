using System;
using MadPixel;

public class MadPixelAdsProvider : IAdsProvider {

    private bool _isShowing;
    private Action _onRewardedShown;
    private Action _onFail;
    
    public void ShowRewardedAd(string placeId, Action onSuccess, Action onFail) {
        if (_isShowing) {
            return;
        }

        _onRewardedShown = onSuccess;
        _onFail = onFail;
        _isShowing = true;
        AdsManager.EResultCode code =  AdsManager.ShowRewarded(MadPixelDDOL.Instance.gameObject, OnFinishAds, placeId);
        if (code != AdsManager.EResultCode.OK)
        {
            _onFail?.Invoke();
            _isShowing = false;
        }
    }

    public bool IsAdsReady() {
        return AdsManager.Ready();
    }

    private void OnFinishAds(bool adShowSuccess)
    {
        if (adShowSuccess)
        {
            _onRewardedShown?.Invoke();
        } else {
            _onFail?.Invoke(); 
        }
        _isShowing = false;
    }
}
