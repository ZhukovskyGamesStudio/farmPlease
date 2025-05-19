using System.Collections.Generic;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimePanelView : MonoBehaviour {
        public CalendarDayView lilCalendarDay;
        public Image CalendarImage;
        public Button CalendarButton;
        public Button HappeningButton;

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
        private HappeningType _currentDay;
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
            int predictedDaysLeft = InventoryManager.ToolsActivated.SafeGet(ToolBuff.Weatherometr, 0);
            for (int i = 0; i < _days.Length; i++) {
                CalendarDayView view = _days[i].GetComponent<CalendarDayView>();
                if (_daysHappenings[i] == HappeningType.Love && !InventoryManager.Instance.IsToolWorking(ToolBuff.Weatherometr))
                    view.SetProps(i, HappeningType.None);
                else if (_daysHappenings[i] != HappeningType.None && _daysHappenings[i] != HappeningType.FoodMarket &&
                         predictedDaysLeft <= 0 && i > curDay)
                    view.SetProps(i, HappeningType.Unknown);
                else
                    view.SetProps(i, _daysHappenings[i]);

                if (i < curDay)
                    view.DayOver();
                else if (i == curDay) {
                    CurrentDay = view.transform;
                    view.DayToday();
                } else {
                    predictedDaysLeft--;
                }
            }
        }

        public void UpdateLilCalendar(int date) {
            _currentDay = _daysHappenings[date];
            lilCalendarDay.SetProps(date, _currentDay, true);
        }

        public void TapOnCurrentDay() {
            Croponom.Instance.OpenOnPage( WeatherTable.WeatherByType( _currentDay).type.ToString());
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