using Managers;
using Tables;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class TimePanel : MonoBehaviour {
        public CalendarDayView LilCalendarDay;
        public Image CalendarImage;
        public Button CalendarButton;
        public Button HappeningButton;

        private HappeningType _currentDay;

        public void UpdateLilCalendar(int date) {
            _currentDay = TimeManager.Days[date];
            LilCalendarDay.SetProps(date, _currentDay, true);
        }

        public void TapOnCurrentDay() {
            UIHud.Instance.Croponom.OpenOnPage(WeatherTable.WeatherByType(_currentDay).type.ToString());
        }

        public void OpenBigCalendar() {
          TimeManager.Instance.ShowBigCalendarDialog();
        }
    }
}