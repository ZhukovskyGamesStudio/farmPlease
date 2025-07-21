using Abstract;

public class ZhukovskyAdsManager: PreloadableSingleton<ZhukovskyAdsManager> {
    
    public IAdsProvider AdsProvider { get; private set; }

    protected override void OnFirstInit() {
        base.OnFirstInit();
        
#if MADPIXEL
         AdsProvider = new MadPixelAdsProvider();
#elif YG_PLATFORM
        AdsProvider = new YGAdsProvider();
#else
        AdsProvider = new AdsProviderMock();
#endif
       
    }
}
