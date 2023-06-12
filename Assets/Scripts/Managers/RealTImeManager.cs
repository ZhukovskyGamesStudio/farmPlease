using System;

namespace Managers
{
    public static class RealTImeManager {
        public const double NEXT_DAY_TIME_POINT = 0;
        public static double EnergyRefillPoint = 12 * 60 * 60;
        public static bool SkipOne;

        public static int MinutesToEnergyRefill() {
            int nowMinutes = TotalMinutes();
            int nextDayMinutes = (int) EnergyRefillPoint / 60;

            if (nowMinutes < nextDayMinutes)
                return nextDayMinutes - nowMinutes;
            return nextDayMinutes + 24 * 60 - nowMinutes;
        }

        public static void ChangeDayPoint(double newPoint, bool byPlayer) {
            if (byPlayer)
                SkipOne = true;
            EnergyRefillPoint = newPoint;
        }

        public static int DaysGone(DateTime old) {
            int daysGone;
            DateTime now = DateTime.Now;
            TimeSpan timeSpan = now - old;

            if (timeSpan <= System.TimeSpan.Zero) return 0;
            daysGone = timeSpan.Days;

            double oldSeconds = old.TimeOfDay.TotalSeconds;
            double nowSeconds = now.TimeOfDay.TotalSeconds;

            if (oldSeconds < NEXT_DAY_TIME_POINT && nowSeconds >= NEXT_DAY_TIME_POINT)
                daysGone++;
            else if (oldSeconds > NEXT_DAY_TIME_POINT && nowSeconds >= NEXT_DAY_TIME_POINT && nowSeconds < oldSeconds)
                daysGone++;
            else if (oldSeconds <= NEXT_DAY_TIME_POINT && nowSeconds < NEXT_DAY_TIME_POINT && nowSeconds < oldSeconds)
                daysGone++;

            return daysGone;
        }

        public static bool IsRefillingEnergy(DateTime old) {
            int daysGone;
            DateTime now = DateTime.Now;
            TimeSpan timeSpan = now - old;

            if (timeSpan <= System.TimeSpan.Zero) return false;
            daysGone = timeSpan.Days;

            double oldSeconds = old.TimeOfDay.TotalSeconds;
            double nowSeconds = now.TimeOfDay.TotalSeconds;

            if (oldSeconds < EnergyRefillPoint && nowSeconds >= EnergyRefillPoint)
                daysGone++;
            else if (oldSeconds > EnergyRefillPoint && nowSeconds >= EnergyRefillPoint && nowSeconds < oldSeconds)
                daysGone++;
            else if (oldSeconds <= EnergyRefillPoint && nowSeconds < EnergyRefillPoint && nowSeconds < oldSeconds)
                daysGone++;
            if (SkipOne && daysGone > 0) {
                SkipOne = false;
                daysGone--;
            }

            return daysGone > 0;
        }

        public static bool IsSkipOne() {
            return SkipOne;
        }

        public static void Skipped() {
            SkipOne = false;
        }

        public static int TotalSeconds() {
            return (int) DateTime.Now.TimeOfDay.TotalSeconds;
        }

        public static int TotalMinutes() {
            return (int) DateTime.Now.TimeOfDay.TotalMinutes;
        }

        public static TimeSpan TimeSpan(DateTime old) {
            DateTime now = DateTime.Now;
            return now - old;
        }

        public static string TimeToString(int minutes) {
            string res = "";
            if (minutes / 60 < 10)
                res += "0";
            res += minutes / 60 + ":";
            if (minutes % 60 < 10)
                res += "0";
            res += minutes % 60;
            return res;
        }
    }
}