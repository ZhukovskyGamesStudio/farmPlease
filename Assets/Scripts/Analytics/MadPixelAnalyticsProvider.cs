using System.Collections.Generic;

public class MadPixelAnalyticsProvider : IAnalyticsProvider {
	public void SendEvent(string eventName, Dictionary<string, object> data) {
		MadPixelAnalytics.AnalyticsManager.CustomEvent(eventName, data);
	}
}