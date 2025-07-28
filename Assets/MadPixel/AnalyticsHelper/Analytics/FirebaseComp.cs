using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseComp : MonoBehaviour {
    #region Fields
    private static bool m_initialized = false;
    #endregion


    #region Public Static
    public static bool Initialized() {
        return (m_initialized);
    }
    #endregion


    #region Unity events
    void Start() {
#if UNITY_EDITOR
        return;
#endif
        InitOnDevice();
    }

    private void InitOnDevice() {
        Debug.LogError($"[FIREBASE] Start init on device");
        //Сюда доходит, внутрь заходит
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            //сюда не доходит
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.LogError($"[FIREBASE] Inited");
                InnerInit();
            } else {
                Debug.LogError($"[FIREBASE] Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    private void OnDestroy() {
        Debug.Log($"[FIREBASE] destroyed");
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= LogAdPurchase;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= LogAdPurchase;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= LogAdPurchase;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent -= LogAdPurchase;
    }
    #endregion

    private void InnerInit() {
        m_initialized = true;
        Debug.Log($"[FIREBASE] subscribed to analytics");
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += LogAdPurchase;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += LogAdPurchase;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += LogAdPurchase;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += LogAdPurchase;
    }

    private void LogAdPurchase(string a_adUnitId, MaxSdkBase.AdInfo a_adInfo) {
        double revenue = a_adInfo.Revenue;
        Debug.Log($"[FIREBASE] LogAdPurchase: {a_adUnitId}. \n{JsonUtility.ToJson(a_adInfo)}");
        if (revenue > 0 && m_initialized) {
            string countryCode = MaxSdk.GetSdkConfiguration()
                        .CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = a_adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = a_adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = a_adInfo.Placement; // The placement this ad's postbacks are tied to
            string networkPlacement = a_adInfo.NetworkPlacement; // The placement ID from the network that showed the ad

            var impressionParameters = new[] {
                new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
                new Firebase.Analytics.Parameter("ad_source", a_adInfo.NetworkName),
                new Firebase.Analytics.Parameter("ad_unit_name", a_adInfo.AdUnitIdentifier),
                new Firebase.Analytics.Parameter("ad_format", a_adInfo.AdFormat),
                new Firebase.Analytics.Parameter("value", revenue),
                new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
            };

            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);

            //Debug.Log($"[MadPixel] Revenue logged {adInfo}");
        }
    }
}