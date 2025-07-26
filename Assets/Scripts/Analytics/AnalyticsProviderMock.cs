using System.Collections.Generic;
using UnityEngine;

public class AnalyticsProviderMock : IAnalyticsProvider {
	public void SendEvent(string eventName, Dictionary<string, object> data) {
		Debug.Log("AnalyticsProviderMock: Sending event: " + eventName);
	}
}