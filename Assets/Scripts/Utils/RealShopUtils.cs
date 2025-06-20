using System;
using Managers;

public class RealShopUtils {
    public static bool IsGoldenClockActive(RealShopData data) {
        return (DateTime.Now - data.GoldenClockLastBoughtDatetime).TotalMinutes < ConfigsManager.Instance.CostsConfig.MinutesGoldenClockWorks;
    }

    public static bool IsGoldenScytheActive(RealShopData data) {
        return (DateTime.Now - data.GoldenScytheLastBoughtDatetime).TotalMinutes < ConfigsManager.Instance.CostsConfig.MinutesGoldenScytheWorks;
    }
}