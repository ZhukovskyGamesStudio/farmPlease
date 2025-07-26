using Abstract;

public class RateUsManager : PreloadableSingleton<RateUsManager> {
    public IRateUsProvider Provider;

    protected override void OnFirstInit() {
        base.OnFirstInit();

#if MADPIXEL
        Provider = new MadPixelRateUsProvider();
#else
        Provider = new RateUsMockProvider();
#endif
    }

    public void TryShowDialog() {
        DialogsManager.Instance.ShowDialog(typeof(RateUsDialog));
    }
}