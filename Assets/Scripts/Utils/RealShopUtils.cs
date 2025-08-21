using System;

public class RealShopUtils {
    public static bool IsAllEndless => true;

    public static bool IsGoldenClockActive(RealShopData data) {
        return ClockTimeLeft(data).TotalSeconds > 0;
    }

    public static TimeSpan ClockTimeLeft(RealShopData data) => data.GoldenClockLastBoughtDatetime + (IsAllEndless
        ? TimeSpan.FromDays(365)
        : TimeSpan.FromMinutes(ConfigsManager.Instance.CostsConfig.MinutesGoldenClockWorks)) - DateTime.Now;

    public static bool IsGoldenScytheActive(RealShopData data) {
        return ScytheTimeLeft(data).TotalSeconds > 0;
    }

    public static TimeSpan ScytheTimeLeft(RealShopData data) => data.GoldenScytheLastBoughtDatetime + (IsAllEndless
        ? TimeSpan.FromDays(365)
        : TimeSpan.FromMinutes(ConfigsManager.Instance.CostsConfig.MinutesGoldenScytheWorks)) - DateTime.Now;
}