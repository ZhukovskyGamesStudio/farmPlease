using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class RealTImeManager
{
    public const double NextDayTimePoint = 0;
    public static double EnergyRefillPoint = 12 * 60 * 60;
    public static bool skipOne;

    public static int MinutesToEnergyRefill()
    {

        int nowMinutes = TotalMinutes();
        int nextDayMinutes = (int)EnergyRefillPoint / 60;

        if (nowMinutes < nextDayMinutes)
        {
            return nextDayMinutes - nowMinutes;
        }
        else
            return nextDayMinutes + 24 * 60 - nowMinutes;
    }

    public static void ChangeDayPoint(double newPoint, bool byPlayer)
    {
        if(byPlayer)
            skipOne = true;
        EnergyRefillPoint = newPoint;
    }

    public static int DaysGone(DateTime old)
    {
        int daysGone;
        DateTime now = DateTime.Now;
        TimeSpan timeSpan = now - old;

        if (timeSpan <= System.TimeSpan.Zero)
        {
            return 0;
        }
        daysGone = timeSpan.Days;

        double oldSeconds = old.TimeOfDay.TotalSeconds;
        double nowSeconds = now.TimeOfDay.TotalSeconds;

        if (oldSeconds < NextDayTimePoint && nowSeconds >= NextDayTimePoint)
            daysGone++;
        else if (oldSeconds > NextDayTimePoint && nowSeconds >= NextDayTimePoint && nowSeconds < oldSeconds)
            daysGone++;
        else if (oldSeconds <= NextDayTimePoint && nowSeconds < NextDayTimePoint && nowSeconds < oldSeconds)
            daysGone++;

        return daysGone;
    }

    public static bool IsRefillingEnergy(DateTime old)
    {
        int daysGone;
        DateTime now = DateTime.Now;
        TimeSpan timeSpan = now - old;

        if (timeSpan <= System.TimeSpan.Zero)
        {
            return false;
        }
        daysGone = timeSpan.Days;

        double oldSeconds = old.TimeOfDay.TotalSeconds;
        double nowSeconds = now.TimeOfDay.TotalSeconds;

        if (oldSeconds < EnergyRefillPoint && nowSeconds >= EnergyRefillPoint)
            daysGone++;
        else if (oldSeconds > EnergyRefillPoint && nowSeconds >= EnergyRefillPoint && nowSeconds < oldSeconds)
            daysGone++;
        else if (oldSeconds <= EnergyRefillPoint && nowSeconds < EnergyRefillPoint && nowSeconds < oldSeconds)
            daysGone++;
        if(skipOne && daysGone > 0)
        {
            skipOne = false;
            daysGone--;
        }


        return daysGone > 0;
    }



    public static bool IsSkipOne()
    {
        return skipOne;
    }

    public static void Skipped()
    {
        skipOne = false;
    }




    public static int TotalSeconds()
    {
        return (int)DateTime.Now.TimeOfDay.TotalSeconds;
    }

    public static int TotalMinutes()
    {
        return (int)DateTime.Now.TimeOfDay.TotalMinutes;
    }

    public static TimeSpan TimeSpan(DateTime old)
    {
        DateTime now = DateTime.Now;
        return now - old;
    }


    public static string TimeToString(int minutes)
    {
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
