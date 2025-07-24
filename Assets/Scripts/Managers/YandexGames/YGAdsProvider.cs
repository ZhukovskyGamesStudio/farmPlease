using System;
using YG;

public class YGAdsProvider : IAdsProvider {

    public void ShowRewardedAd(string placeId, Action onSuccess, Action onFail) {
        YG2.RewardedAdvShow(placeId, onSuccess);
    }

    public void ShowInterAd(string placeId, Action onSuccess = null, Action onFail = null) {
        throw new NotImplementedException();
    }

    public bool IsAdsReady() {
        return true;
    }
}