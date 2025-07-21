using System;
using YG;

public class YGAdsProvider : IAdsProvider {

    public void ShowRewardedAd(string placeId, Action onSuccess, Action onFail) {
        YG2.RewardedAdvShow(placeId, onSuccess);
    }
    
    public bool IsAdsReady() {
        return true;
    }
}