using System;

public class AdsProviderMock :IAdsProvider {
    public void ShowRewardedAd(string placeId, Action onSuccess, Action onFail) {
        onSuccess?.Invoke();
    }
}
