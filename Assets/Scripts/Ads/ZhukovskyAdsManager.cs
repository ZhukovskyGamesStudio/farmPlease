using System;
using Abstract;
using Managers;

public class ZhukovskyAdsManager : PreloadableSingleton<ZhukovskyAdsManager> {
    public float GameInterAdCooldown;
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

        if (SaveLoadManager.CurrentSave.RealShopData.HasNoAds) {
            AdsProvider.CancelAds();
        }

        InterAdRunner = new InterAdRunner(GameInterAdCooldown, AdsProvider);
        if (LevelsUtils.IsIntersUnlocked) {
            InterAdRunner.IsInterAdRunEnabled = true;
        }
    }

    private void Update() {
        InterAdRunner.Update();
    }
}