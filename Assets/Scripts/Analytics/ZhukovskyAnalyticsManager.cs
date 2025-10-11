using System.Collections.Generic;
using Abstract;
using JetBrains.Annotations;

public class ZhukovskyAnalyticsManager : PreloadableSingleton<ZhukovskyAnalyticsManager> {
    [ItemCanBeNull]
    public List<IAnalyticsProvider> AnalyticProviders { get; private set; } = new List<IAnalyticsProvider>();

    protected override void OnFirstInit() {
        base.OnFirstInit();
        AnalyticProviders.Add(new AnalyticsProviderMock());
#if MADPIXEL
        AnalyticProviders.Add(new MadPixelAnalyticsProvider());
#endif
    }

    public void SendCustomEvent(string eventName, Dictionary<string, object> data, bool bSendEventBuffer = false) {
        foreach (IAnalyticsProvider analyticProvider in AnalyticProviders) {
            analyticProvider.SendEvent(eventName, data, bSendEventBuffer);
        }
    }
}