using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Managers;

public class RateUsDialog : DialogBase {
    private bool _isWaitingReview;

    protected override bool IsHideProfile => true;

    public void RateGood() {
        if (_isWaitingReview) {
            return;
        }
        
        _isWaitingReview = true;
        SendRateUsEvent(5);
        SaveLoadManager.CurrentSave.WasRated = true;
        SaveLoadManager.SaveGame();
        LaunchReviewFlow().Forget();
    }

    public void RateBad() {
        SendRateUsEvent(1);
        SaveLoadManager.CurrentSave.LastTimeRateUsShowed = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        SaveLoadManager.SaveGame();
        CloseByButton();
    }

    public void CloseWithEvent() {
        SendRateUsEvent(0);
        CloseByButton();
    }
    private async UniTask LaunchReviewFlow() {
        await RateUsManager.Instance.Provider.Show();

        CloseByButton();
    }

    private void SendRateUsEvent(int rateResult) {
        ZhukovskyAnalyticsManager.Instance.SendCustomEvent("rate_us", new Dictionary<string, object>{
            {"show_reason", RateUsManager.Instance.RateUsSource},
            {"rate_result", rateResult}
        });
        RateUsManager.Instance.RateUsSource = null;
    }
}