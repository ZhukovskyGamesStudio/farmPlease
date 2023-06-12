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
    private GameObject[] _days;
    private int _daysAmount;
    private List<HappeningType> _daysHappenings;
    private GameObject[] _skipDays;

    private int _skipDaysAmount;

    public void CreateDays(List<HappeningType> daysHappenings, int skipAmount) {
        _daysHappenings = daysHappenings;
        _daysAmount = daysHappenings.Count;
        _skipDaysAmount = skipAmount;

        if (_skipDays != null)
            for (int i = 0; i < _skipDays.Length; i++)
                Destroy(_skipDays[i]);

        if (_skipDaysAmount > 0) {
            _skipDays = new GameObject[_skipDaysAmount];
            for (int i = 0; i < _skipDaysAmount; i++) {
                _skipDays[i] = Instantiate(DayPref, DaysParent.transform);
                _skipDays[i].GetComponent<DayScript>().Clear();
            }
        }

        if (_days != null)
            for (int i = 0; i < _days.Length; i++)
                Destroy(_days[i]);

        _days = new GameObject[_daysAmount];
        for (int i = 0; i < _daysAmount; i++) {
            _days[i] = Instantiate(DayPref, DaysParent.transform);
            _days[i].GetComponent<DayScript>().SetProps(i, daysHappenings[i]);
        }
    }

    private void UpdateBigCalendar(int curDay) {
        for (int i = 0; i < _days.Length; i++) {
            DayScript script = _days[i].GetComponent<DayScript>();
            if (_daysHappenings[i] == HappeningType.Love &&
                !InventoryManager.Instance.IsToolWorking(ToolBuff.Weatherometr))
                script.SetProps(i, HappeningType.None);
            else if (_daysHappenings[i] != HappeningType.None && _daysHappenings[i] != HappeningType.Marketplace &&
                     !InventoryManager.Instance.IsToolWorking(ToolBuff.Weatherometr))
                script.SetProps(i, HappeningType.Unknown);
            else
                script.SetProps(i, _daysHappenings[i]);

            if (i < curDay)
                script.DayOver();
        }
    }

    public void UpdateLilCalendar(int date) {
        if (_daysHappenings[date] != HappeningType.None && _daysHappenings[date] != HappeningType.Marketplace &&
            !InventoryManager.Instance.IsToolWorking(ToolBuff.Weatherometr))
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