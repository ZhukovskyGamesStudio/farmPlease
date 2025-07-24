using System;

public class AdsProviderMock :IAdsProvider {
    public void ShowRewardedAd(string placeId, Action onSuccess, Action onFail) {
        onSuccess?.Invoke();
    }

    public void ShowInterAd(string placeId, Action onSuccess = null, Action onFail = null) {
        throw new NotImplementedException();
    }

    public bool IsAdsReady() {
        return true;
    }
}
