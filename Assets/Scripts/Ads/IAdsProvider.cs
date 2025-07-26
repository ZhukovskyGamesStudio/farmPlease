﻿using System;

public interface IAdsProvider {
    public void ShowRewardedAd(string placeId, Action onSuccess, Action onFail);
    public void ShowInterAd(string placeId, Action onSuccess = null, Action onFail = null);

    public bool IsAdsReady();

    public void CancelAds();
}
