using System;
using System.Collections.Generic;
using System.Globalization;
using Dialogs;
using Managers;
using Tables;
using TMPro;
using UI;
using UnityEngine;

public class BigCalendarDialog : Dialogs.DialogWithData<BigCalendarData> {
    private List<CalendarDayView> _days;
    private List<CalendarDayView> _skippedDays;
    public CalendarDayView DayPref;
    public Transform DaysParent;
    private int _curDay;

    [SerializeField]
    private GameObject _adButton;

    [SerializeField]
    private TextMeshProUGUI _monthText;

    public override void SetData(BigCalendarData data) {
        CreateDaysViews(data.DaysHappenings, data.SkipAmount);
        UpdateBigCalendar(SaveLoadManager.CurrentSave.CurrentDayInMonth);
        int predictedDaysLeft = InventoryManager.ToolsActivated.SafeGet(ToolBuff.Weatherometr, 0);

        _adButton.gameObject.SetActive(predictedDaysLeft == 0 && _days.Count - _curDay >= 7);

        string[] months = new string[12] {
            "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december"
        };
        var date = DateTime.Parse(SaveLoadManager.CurrentSave.Date, CultureInfo.InvariantCulture);
        int curMonth = DateTime.Parse(SaveLoadManager.CurrentSave.Date, CultureInfo.InvariantCulture).Month;
        string localizedMonth = ZG_Localization.LocalizationManager.Instance.GetText(months[curMonth - 1]);
        _monthText.text = $"{localizedMonth} . {date.Year}";
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
        _curDay = curDay;
        int predictedDaysLeft = InventoryManager.ToolsActivated.SafeGet(ToolBuff.Weatherometr, 0);
        for (int i = 0; i < _days.Count; i++) {
            CalendarDayView view = _days[i].GetComponent<CalendarDayView>();
            if (TimeManager.Days[i] == HappeningType.Love && predictedDaysLeft > 0) {
                view.SetProps(i, HappeningType.NormalSunnyDay);
            } else if (TimeManager.Days[i] == HappeningType.Unknown && predictedDaysLeft > 0 && i > curDay) {
                TimeManager.UnveilUnknownHappening(i, true);
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

    public void KnowAllWeatherAd() {
        DialogsManager.Instance.ShowDialogWithData(typeof(WatchAdDialog), new WatchAdDialog.Data() {
            Reward = new Reward() {
                Items = new List<RewardItem>() {
                    new RewardItem() {
                        Amount = 1,
                        Type = nameof(AdRewards.KnowAllWeather)
                    }
                }
            },
            Header = ZG_Localization.LocalizationManager.Instance.GetText("know_weather_ad_header"),
            OnClaim = () => {
                InventoryManager.Instance.ActivateTool(ToolBuff.Weatherometr);
                SaveLoadManager.CurrentSave.ToolBuffs[ToolBuff.Weatherometr] = _days.Count - _curDay + 1;
                TimeManager.Instance.ShowBigCalendarDialog();
            },
            IsJustImage = true,
            JustSprite = ConfigsManager.Instance.AdRewardsConfig.AdRewardIcons[AdRewards.KnowAllWeather],
            AdId = AdsIds.RewardedWeatherAhead
        });
        CloseByButton();
    }
}

[Serializable]
public class BigCalendarData {
    public List<HappeningType> DaysHappenings;
    public int SkipAmount;
}