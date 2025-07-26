using Abstract;
using UnityEngine.Analytics;

public class ZhukovskyAnalyticsManager : PreloadableSingleton<ZhukovskyAnalyticsManager> {
	
	public IAnalyticsProvider AnalyticProvider { get; private set; }
	protected override void OnFirstInit() {
		base.OnFirstInit();
#if MADPIXEL
		AnalyticProvider = new MadPixelAnalyticsProvider();
#else		
		AnalyticProvider = new AnalyticsProviderMock();
#endif	
	}
}