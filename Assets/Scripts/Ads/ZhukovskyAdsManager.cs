using Abstract;

public class ZhukovskyAdsManager: PreloadableSingleton<ZhukovskyAdsManager> {
    
    public IAdsProvider AdsProvider { get; private set; }

    protected override void OnFirstInit() {
        base.OnFirstInit();
        
#if MADPIXEL
         AdsProvider = new MadPixelAdsProvider();
#else
        AdsProvider = new AdsProviderMock();
#endif
       
    }
}
