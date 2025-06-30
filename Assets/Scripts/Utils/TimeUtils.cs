using System;
using Localization;

public static class TimeUtils {
    public static string ToShortString(TimeSpan timeSpan) {
        if (Math.Floor(timeSpan.TotalHours) > 0) {
            return $"{timeSpan.Hours}{LocalizationUtils.L("hour")} {timeSpan.Minutes}{LocalizationUtils.L("minute")} ";
        }

        if (Math.Floor(timeSpan.TotalMinutes) > 0) {
            return $"{timeSpan.Minutes}{LocalizationUtils.L("minute")}  {timeSpan.Seconds}{LocalizationUtils.L("second")} ";
        }

        return $"{timeSpan.Seconds}{LocalizationUtils.L("second")} ";
    }
}