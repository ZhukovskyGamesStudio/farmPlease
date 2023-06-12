using System.Collections.Generic;
using DefaultNamespace.Managers;
using UnityEngine;
using UnityEngine.UI;

public class TimePanel : MonoBehaviour {
    public DayScript lilDay;
    public Image CalendarImage;

    public GameObject CalendarPanel;
    public GameObject DayPref;
    public GameObject DaysParent;
    public GameObject timeHintPanel;
    public Text TimeOfDayText;
    public bool isOpen;
    private GameObject[] Days;
    private int DaysAmount;
    private List<HappeningType> _daysHappenings;
    private GameObject[] SkipDays;

    private int SkipDaysAmount;

    public void CreateDays(List<HappeningType> daysHappenings, int skipAmount) {
        _daysHappenings = daysHappenings;
        DaysAmount = daysHappenings.Count;
        SkipDaysAmount = skipAmount;

        if (SkipDays != null)
            for (int i = 0; i < SkipDays.Length; i++)
                Destroy(SkipDays[i]);

        if (SkipDaysAmount > 0) {
            SkipDays = new GameObject[SkipDaysAmount];
            for (int i = 0; i < SkipDaysAmount; i++) {
                SkipDays[i] = Instantiate(DayPref, DaysParent.transform);
                SkipDays[i].GetComponent<DayScript>().Clear();
            }
        }

        if (Days != null)
            for (int i = 0; i < Days.Length; i++)
                Destroy(Days[i]);

        Days = new GameObject[DaysAmount];
        for (int i = 0; i < DaysAmount; i++) {
            Days[i] = Instantiate(DayPref, DaysParent.transform);
            Days[i].GetComponent<DayScript>().SetProps(i, daysHappenings[i]);
        }
    }

    private void UpdateBigCalendar(int curDay) {
        for (int i = 0; i < Days.Length; i++) {
            DayScript script = Days[i].GetComponent<DayScript>();
            if (_daysHappenings[i] == HappeningType.Love &&
                !InventoryManager.instance.IsToolWorking(ToolBuff.Weatherometr))
                script.SetProps(i, HappeningType.None);
            else if (_daysHappenings[i] != HappeningType.None && _daysHappenings[i] != HappeningType.Marketplace &&
                     !InventoryManager.instance.IsToolWorking(ToolBuff.Weatherometr))
                script.SetProps(i, HappeningType.Unknown);
            else
                script.SetProps(i, _daysHappenings[i]);

            if (i < curDay)
                script.DayOver();
        }
    }

    public void UpdateLilCalendar(int date) {
        if (_daysHappenings[date] != HappeningType.None && _daysHappenings[date] != HappeningType.Marketplace &&
            !InventoryManager.instance.IsToolWorking(ToolBuff.Weatherometr))
            lilDay.SetProps(date, HappeningType.Unknown);
        else
            lilDay.SetProps(date, _daysHappenings[date], true);
    }

    public void CalendarPanelOpenClose() {
        isOpen = !isOpen;
        CalendarPanel.SetActive(isOpen);
        if (isOpen) {
            UpdateBigCalendar(SaveLoadManager.CurrentSave.CurrentDay);
        }
    }
}