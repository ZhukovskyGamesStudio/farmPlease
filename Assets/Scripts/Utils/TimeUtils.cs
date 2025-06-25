using System;

public static class TimeUtils {
    public static string ToShortString(TimeSpan timeSpan) {
        if (Math.Floor(timeSpan.TotalHours) > 0) {
            return $"{timeSpan.Hours}h {timeSpan.Minutes}m";
        }

        if (Math.Floor(timeSpan.TotalMinutes) > 0) {
            return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
        }

        return $"{timeSpan.Seconds}s";
    }
}