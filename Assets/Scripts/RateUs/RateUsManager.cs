using System;
using System.Globalization;
using Abstract;
using Managers;

public class RateUsManager : PreloadableSingleton<RateUsManager> {
    public IRateUsProvider Provider;
    private DateTime _lastRateUsShowed;
    private bool _wasRated;
    protected override void OnFirstInit() {
        base.OnFirstInit();

#if MADPIXEL
        Provider = new MadPixelRateUsProvider();
#else
        Provider = new RateUsMockProvider();
#endif
	    _lastRateUsShowed = DateTime.Parse(SaveLoadManager.CurrentSave.LastTimeRateUsShowed, CultureInfo.InvariantCulture);
	    _wasRated = SaveLoadManager.CurrentSave.WasRated;
    }

    public void TryShowDialog() {
	    if (_wasRated) return;
	    if ((DateTime.Now - _lastRateUsShowed).Days < 3) return;
        DialogsManager.Instance.ShowDialog(typeof(RateUsDialog));
    }
}