using System;
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
        SaveLoadManager.CurrentSave.WasRated = true;
        LaunchReviewFlow().Forget();
    }

    public void RateBad() {
        SaveLoadManager.CurrentSave.LastTimeRateUsShowed = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        CloseByButton();
    }

    private async UniTask LaunchReviewFlow() {
        await RateUsManager.Instance.Provider.Show();

        CloseByButton();
    }
}