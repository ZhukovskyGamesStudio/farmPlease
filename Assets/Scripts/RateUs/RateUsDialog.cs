using Cysharp.Threading.Tasks;

public class RateUsDialog : DialogBase {
    private bool _isWaitingReview;

    protected override bool IsHideProfile => true;

    public void RateGood() {
        if (_isWaitingReview) {
            return;
        }

        _isWaitingReview = true;
        LaunchReviewFlow().Forget();
    }

    public void RateBad() {
        CloseByButton();
    }

    private async UniTask LaunchReviewFlow() {
        await RateUsManager.Instance.Provider.Show();

        CloseByButton();
    }
}