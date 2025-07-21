using System;

public interface IAdsProvider {
    public void ShowRewardedAd(string placeId, Action onSuccess, Action onFail);

    public bool IsAdsReady();
}
