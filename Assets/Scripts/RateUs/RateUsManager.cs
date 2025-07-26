using System;
using System.Globalization;
using Abstract;
using Managers;

public class RateUsManager : PreloadableSingleton<RateUsManager> {
    public IRateUsProvider Provider;
    public int RateUsCooldown;
    

    protected override void OnFirstInit() {
        base.OnFirstInit();

#if MADPIXEL
        Provider = new MadPixelRateUsProvider();
#else
        Provider = new RateUsMockProvider();
#endif

    }

    public void TryShowDialog() {
	    var lastRateUsShowed = DateTime.Parse(SaveLoadManager.CurrentSave.LastTimeRateUsShowed, CultureInfo.InvariantCulture);
	    var wasRated = SaveLoadManager.CurrentSave.WasRated;
	    if (wasRated) return;
	    if ((DateTime.Now - lastRateUsShowed).Days < RateUsCooldown) return;
        DialogsManager.Instance.ShowDialog(typeof(RateUsDialog));
    }
}