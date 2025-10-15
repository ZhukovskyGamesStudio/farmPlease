using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dialogs;
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

        [SerializeField]
        private Button _upgradeWeatherButton;

        private List<HappeningType> _happeningWithoutUpgrade = new List<HappeningType>() {
            HappeningType.NormalSunnyDay,
            HappeningType.FoodMarket,
            HappeningType.Love,
        };

        public void UpdateLilCalendar(int date) {
            _currentDay = TimeManager.Days[date];
            LilCalendarDay.SetProps(date, _currentDay, true);

            _upgradeWeatherButton.gameObject.SetActive(!_happeningWithoutUpgrade.Contains(_currentDay));
        }

        public void TapOnCurrentDay() {
            UIHud.Instance.Croponom.OpenOnPage(WeatherTable.WeatherByType(_currentDay).type.ToString());
        }

        public void WatchAdForWeatherUpgrade() {
            DialogsManager.Instance.ShowDialogWithData(typeof(WatchAdDialog), new WatchAdDialog.Data() {
                Reward = new Reward() {
                    Items = new List<RewardItem>() {
                        new RewardItem() {
                            Amount = 1,
                            Type = nameof(AdRewards.UpgradeWeather)
                        }
                    }
                },
                Header = ZG_Localization.LocalizationManager.Instance.GetText("better_weather_ad_header"),
                OnClaim = () => {
                    SaveLoadManager.CurrentSave.Days[SaveLoadManager.CurrentSave.CurrentDayInMonth] = HappeningType.NormalSunnyDay;
                    UpdateLilCalendar(SaveLoadManager.CurrentSave.CurrentDayInMonth);
                    SmartTilemap.Instance.SetHappeningType(HappeningType.NormalSunnyDay);
                    UIHud.Instance.screenEffect.ChangeEffectCoroutine(HappeningType.NormalSunnyDay, false).Forget();
                },
                IsJustImage = true,
                JustSprite = ConfigsManager.Instance.AdRewardsConfig.AdRewardIcons[AdRewards.UpgradeWeather],
                AdId = AdsIds.RewardedBetterWeather
            });
        }

        public void OpenBigCalendar() {
            TimeManager.Instance.ShowBigCalendarDialog();
        }
    }
}