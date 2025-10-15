using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class CalendarDayView : MonoBehaviour {
        public Image Happening;
        public GameObject Finished;
        public GameObject Today;
        public GameObject TodayFrame;

        [SerializeField]
        private GameObject _blank;

        [SerializeField]
        private CanvasGroup _dateCanvasGroup;

        [SerializeField]
        private TextMeshProUGUI _dateText;

        public void Clear() {
            _blank.SetActive(true);
            Happening.gameObject.SetActive(false);
            Finished.SetActive(false);
            Today.SetActive(false);
            TodayFrame.SetActive(false);
        }

        public void DayToday() {
            Today.SetActive(true);
            TodayFrame.SetActive(true);
            Finished.SetActive(false);
            _dateCanvasGroup.alpha = 1;
            //Happening.color = new Color(1, 1, 1, 1);
        }

        public void DayOver() {
            Finished.SetActive(true);
            Today.SetActive(false);
            TodayFrame.SetActive(false);
            _dateCanvasGroup.alpha = 0.5f;
            //Happening.color = new Color(1, 1, 1, 0.5f);
        }

        public void DayFuture() {
            Finished.SetActive(false);
            Today.SetActive(false);
            TodayFrame.SetActive(false);
            _dateCanvasGroup.alpha = 1;
            //Happening.color = new Color(1, 1, 1, 1);
        }

        public void SetProps(int dayNumber, HappeningType type, bool showDefault = false) {
            if (dayNumber == -1) {
                Clear();
                return;
            }

            _dateText.text = (dayNumber + 1).ToString();
            SetHappening(type, showDefault);
        }

        private void SetHappening(HappeningType type, bool showDefault = false) {
            Happening.gameObject.SetActive(true);
            Happening.sprite = WeatherTable.WeatherByType(type).DaySprite;

            if (type == HappeningType.NormalSunnyDay && !showDefault) {
                Happening.gameObject.SetActive(false);
                return;
            }

            Happening.gameObject.SetActive(true);
            Happening.sprite = WeatherTable.WeatherByType(type).DaySprite;
        }
    }
}