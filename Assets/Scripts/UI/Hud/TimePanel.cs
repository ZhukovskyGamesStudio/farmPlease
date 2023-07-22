using System.Collections.Generic;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimePanel : MonoBehaviour {
        public CalendarDayView lilCalendarDay;
        public Image CalendarImage;
        public Button CalendarButton;

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
        public Transform CurrentDay { get; private set; }

        public void CreateDays(List<HappeningType> daysHappenings, int skipAmount) {
            _daysHappenings = daysHappenings;
            _daysAmount = daysHappenings.Count;
            _skipDaysAmount = skipAmount-1;

            if (_skipDays != null)
                for (int i = 0; i < _skipDays.Length; i++)
                    Destroy(_skipDays[i]);

            if (_skipDaysAmount > 0) {
                _skipDays = new GameObject[_skipDaysAmount];
                for (int i = 0; i < _skipDaysAmount; i++) {
                    _skipDays[i] = Instantiate(DayPref, DaysParent.transform);
                    _skipDays[i].GetComponent<CalendarDayView>().Clear();
                }
            }

            if (_days != null)
                for (int i = 0; i < _days.Length; i++)
                    Destroy(_days[i]);

            _days = new GameObject[_daysAmount];
            for (int i = 0; i < _daysAmount; i++) {
                _days[i] = Instantiate(DayPref, DaysParent.transform);
                _days[i].GetComponent<CalendarDayView>().SetProps(i, daysHappenings[i]);
            }
        }

        private void UpdateBigCalendar(int curDay) {
            for (int i = 0; i < _days.Length; i++) {
                CalendarDayView view = _days[i].GetComponent<CalendarDayView>();
                if (_daysHappenings[i] == HappeningType.Love &&
                    !InventoryManager.Instance.IsToolWorking(ToolBuff.Weatherometr))
                    view.SetProps(i, HappeningType.None);
                else if (_daysHappenings[i] != HappeningType.None && _daysHappenings[i] != HappeningType.Marketplace &&
                         !InventoryManager.Instance.IsToolWorking(ToolBuff.Weatherometr) && i > curDay)
                    view.SetProps(i, HappeningType.Unknown);
                else
                    view.SetProps(i, _daysHappenings[i]);

                if (i < curDay)
                    view.DayOver();
                if (i == curDay) {
                    CurrentDay = view.transform;
                    view.DayToday();
                }
                   
            }
        }

        public void UpdateLilCalendar(int date) {
            lilCalendarDay.SetProps(date, _daysHappenings[date], true);
        }

        public void CalendarPanelOpenClose() {
            isOpen = !isOpen;
            CalendarPanel.SetActive(isOpen);
            if (isOpen) {
                UpdateBigCalendar(SaveLoadManager.CurrentSave.CurrentDay);
            }
        }
    }
}