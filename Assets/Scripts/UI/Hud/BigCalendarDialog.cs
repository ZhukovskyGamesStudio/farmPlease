using System;
using System.Collections.Generic;
using Managers;
using Tables;
using UI;
using UnityEngine;

public class BigCalendarDialog : DialogWithData<BigCalendarData> {
    private List<CalendarDayView> _days;
    private List<CalendarDayView> _skippedDays;
    public CalendarDayView DayPref;
    public Transform DaysParent;

    public override void SetData(BigCalendarData data) {
        CreateDaysViews(data.DaysHappenings, data.SkipAmount);
        UpdateBigCalendar(SaveLoadManager.CurrentSave.CurrentDayInMonth);
    }

    private void CreateDaysViews(List<HappeningType> daysHappenings, int skipAmount) {
        int skipDaysAmount = skipAmount - 1;
        if (skipDaysAmount > 0) {
            _skippedDays = new List<CalendarDayView>();
            for (int i = 0; i < skipDaysAmount; i++) {
                CalendarDayView go = Instantiate(DayPref, DaysParent.transform);
                go.Clear();
                _skippedDays.Add(go);
            }
        }

        _days = new List<CalendarDayView>();
        for (int i = 0; i < daysHappenings.Count; i++) {
            var go = Instantiate(DayPref, DaysParent.transform);
            go.SetProps(i, daysHappenings[i]);
            _days.Add(go);
        }
    }

    private void UpdateBigCalendar(int curDay) {
        int predictedDaysLeft = InventoryManager.ToolsActivated.SafeGet(ToolBuff.Weatherometr, 0);
        for (int i = 0; i < _days.Count; i++) {
            CalendarDayView view = _days[i].GetComponent<CalendarDayView>();
            if (TimeManager.Days[i] == HappeningType.Love && predictedDaysLeft > 0) {
                view.SetProps(i, HappeningType.NormalSunnyDay);
            } else if (TimeManager.Days[i] == HappeningType.Unknown && predictedDaysLeft > 0 && i > curDay) {
                TimeManager.UnveilUnknownHappening(i);
                view.SetProps(i, TimeManager.Days[i]);
            } else {
                view.SetProps(i, TimeManager.Days[i]);
            }

            if (i < curDay)
                view.DayOver();
            else if (i == curDay) {
                view.DayToday();
            } else {
                predictedDaysLeft--;
                view.DayFuture();
            }
        }
    }
}

[Serializable]
public class BigCalendarData {
    public List<HappeningType> DaysHappenings;
    public int SkipAmount;
}