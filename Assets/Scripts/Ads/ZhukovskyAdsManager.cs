using System;
using Abstract;

public class ZhukovskyAdsManager: PreloadableSingleton<ZhukovskyAdsManager> {
	public float GameInterAdCooldown;
	public int DayWhenInterAdEnabled;
	public IAdsProvider AdsProvider { get; private set; }
	public InterAdRunner InterAdRunner { get; private set; }

	protected override void OnFirstInit() {
		base.OnFirstInit();
#if MADPIXEL
         AdsProvider = new MadPixelAdsProvider();
#elif YG_PLATFORM
        AdsProvider = new YGAdsProvider();
#else
		AdsProvider = new AdsProviderMock();
#endif
       
		InterAdRunner = new InterAdRunner(GameInterAdCooldown, AdsProvider);
	}

	private void Update() { 
		InterAdRunner.Update();
	}
}