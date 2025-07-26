using System;
using UnityEngine;

public class AdsProviderMock : IAdsProvider {
    private bool _isAdsCancelled;

    public void ShowRewardedAd(string placeId, Action onSuccess, Action onFail) {
        onSuccess?.Invoke();
    }

    public void ShowInterAd(string placeId, Action onSuccess = null, Action onFail = null) {
        if (_isAdsCancelled) {
            Debug.Log("Ads cancelled, no interstitial!");
        } else {
            Debug.Log("Showing interstitial!");
        }
    }

    public bool IsAdsReady() {
        return true;
    }

    public void CancelAds() {
        _isAdsCancelled = true;
        Debug.Log("CancelAds called!");
    }
}