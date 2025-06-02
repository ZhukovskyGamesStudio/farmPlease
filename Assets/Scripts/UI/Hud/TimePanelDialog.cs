using System;
using System.Collections.Generic;
using Managers;
using Tables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimePanelDialog : MonoBehaviour {
        public CalendarDayView lilCalendarDay;
        public Image CalendarImage;
        public Button CalendarButton;
        public Button HappeningButton;
        
        public GameObject DayPref;
        public GameObject DaysParent;
        public GameObject timeHintPanel;
        public Text TimeOfDayText;
        public bool isOpen;
        private GameObject[] _days;
        private int _daysAmount;
        private GameObject[] _skipDays;
        private HappeningType _currentDay;
        private int _skipDaysAmount;
        public Transform CurrentDay { get; private set; }

        private Action _onClose;
        
        public void Show(Action onClose) {
            _onClose = onClose;
            var date = SaveLoadManager.CurrentSave.ParsedDate;
            var skipDaysAmount = TimeManager.FirstDayInMonth(date.Year, date.Month);
            CreateDaysViews(SaveLoadManager.CurrentSave.Days, skipDaysAmount);
        }

        public void Close() {
            _onClose?.Invoke();
        }

        public void CreateDaysViews(List<HappeningType> daysHappenings, int skipAmount) {
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

        public void UpdateLilCalendar(int date) {
            _currentDay = TimeManager.Days[date];
            lilCalendarDay.SetProps(date, _currentDay, true);
        }

        public void TapOnCurrentDay() {
            UIHud.Instance.Croponom.OpenOnPage( WeatherTable.WeatherByType( _currentDay).type.ToString());
        }
        
        public void OpenBigCalendar() {
            DialogsManager.Instance.ShowBigCalendarDialog(null);
        }
    }
}