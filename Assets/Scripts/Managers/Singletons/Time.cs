using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using Tables;
using Training;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

namespace Managers {
    public class Time : Singleton<Time> {
        public static DateTime FirstDayOfGame => new DateTime(2051, 5, 1);

        [Range(1, 31)]
        public int MaxDays;

        [Range(0, 7)]
        public int SkipDaysAmount;

        public EndTrainingPanel EndTrainingPanel;
        public EndMonthPanel EndMonthPanel;

        [SerializeField]
        private SpotlightAnimConfig _calendarHint, _weatherHint, _happeningHint, _foodMarketHint;

        public List<HappeningType> Days => SaveLoadManager.CurrentSave.Days;

        public float SessionTime;
        
        private bool _isTimerWorking;
        private PlayerController PlayerController => global::PlayerController.Instance;
        private SeedShopView SeedShopView => UIHud.ShopsPanel.seedShopView;
        private SmartTilemap SmartTilemap => SmartTilemap.Instance;
        private TimePanel TimePanel => UIHud.TimePanel;
        private ToolShopView ToolShop => UIHud.ShopsPanel.toolShopView;

        private UIHud UIHud => global::UI.UIHud.Instance;

        /**********/
        protected override void OnFirstInit() {
            _isTimerWorking = false;
            SessionTime = 0;
        }

        private void Update() {
            SessionTime += UnityEngine.Time.deltaTime;
        }

        /**********/

        public bool IsTodayLoveDay => Days[SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.Love;

        public void SetDaysWithData(List<HappeningType> daysData, DateTime date) {
            ChangeDayPoint(Settings.Instance.GetDayPoint());

            if (daysData == null) {
                GenerateDays(0);
                return;
            }

            TryChangeMonth();

            MaxDays = daysData.Count;
            SkipDaysAmount = FirstDayInMonth(date.Year, date.Month);

            TimePanel.gameObject.SetActive(SaveLoadManager.CurrentSave.KnowledgeList.Contains(Knowledge.Weather));
            TimePanel.CreateDays(Days, SkipDaysAmount);
            TimePanel.UpdateLilCalendar(SaveLoadManager.CurrentSave.CurrentDay);
            SmartTilemap.SetHappeningType(Days[SaveLoadManager.CurrentSave.CurrentDay]);
            if (GameModeManager.Instance.GameMode != GameMode.Training) {
                if (Days[SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.FoodMarket) {
                    UIHud.OpenBuildingsShop();
                } else {
                    UIHud.CloseBuildingsShop();
                }
            }

            StartCoroutine(UIHud.screenEffect.SetEffectCoroutine(Days[SaveLoadManager.CurrentSave.CurrentDay], false));
        }

        public void GenerateDays(int month) {
            DateTime date = FirstDayOfGame.AddMonths(month);
            MaxDays = DateTime.DaysInMonth(date.Year, date.Month);
            SkipDaysAmount = FirstDayInMonth(date.Year, date.Month);
            ChangeDayPoint(Settings.Instance.GetDayPoint());

            SaveLoadManager.CurrentSave.CurrentDay = 0;

            GenerateHappenings();

            TimePanel.CreateDays(Days, SkipDaysAmount);

            TimePanel.UpdateLilCalendar(SaveLoadManager.CurrentSave.CurrentDay);
            ToolShop.ChangeToolsNewDay();

            if (GameModeManager.Instance.GameMode != GameMode.Training) {
                if (Days[SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.FoodMarket) {
                    UIHud.OpenBuildingsShop();
                } else {
                    UIHud.CloseBuildingsShop();
                }
            }
        }

        private void GenerateHappenings() {
            SaveLoadManager.CurrentSave.Days = new List<HappeningType>();
            for (int i = 0; i < MaxDays; i++) {
                Days.Add(HappeningType.None);
            }

            int love = -1;
            bool isTrainingRainNext = SaveLoadManager.CurrentSave.CurrentMonth == 0 && SaveLoadManager.CurrentSave.CurrentDay == 0;
            while ((love + SkipDaysAmount) % 7 == 0 || (love + 1) % 5 == 0) love = Random.Range(7 + SkipDaysAmount, 20);

            for (int i = 0; i < MaxDays; i++) {
                int x = i + SkipDaysAmount;
                if (x % 7 == 0 && x > 0 && GameModeManager.Instance.GameMode != GameMode.Training) {
                    Days[i] = HappeningType.FoodMarket;
                } else if ((i + 1) % 5 == 0) {
                    if (isTrainingRainNext) {
                        Days[i] = HappeningType.Rain;
                        isTrainingRainNext = false;
                        continue;
                    }

                    int rnd = Random.Range(0, 4);

                    Days[i] = rnd switch {
                        0 => HappeningType.Rain,
                        1 => HappeningType.Erosion,
                        2 => HappeningType.Wind,
                        3 => HappeningType.Insects,
                        _ => Days[i]
                    };
                } else if (i == love) {
                    Days[i] = HappeningType.Love;
                }
            }
        }

        public void AddDay() {
            SaveLoadManager.CurrentSave.CurrentDay++;
            TryChangeMonth();
            StartCoroutine(DayPointCoroutine());
            Energy.Instance.RestoreEnergy();
            if (!SaveLoadManager.CurrentSave.KnowledgeList.Contains(Knowledge.Weather)) {
                TryShowCalendarHint();
            }

            if (!SaveLoadManager.CurrentSave.KnowledgeList.Contains(Knowledge.LilCalendar)) {
                TryShowHappeningHint();
            }
            
            if (!SaveLoadManager.CurrentSave.KnowledgeList.Contains(Knowledge.FoodMarket)) {
                TryShowFoodMarketHint();
            }
        }

        private void TryShowCalendarHint() {
            int nextDay = SaveLoadManager.CurrentSave.CurrentDay + 1;
            if (Days.Count > nextDay && Days[nextDay] != HappeningType.None) {
                UIHud.TimePanel.gameObject.SetActive(true);
                UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.TimePanel.CalendarButton, _calendarHint, ShowWeatherHint);
            }
        }

        private void TryShowHappeningHint() {
            if (Days[SaveLoadManager.CurrentSave.CurrentDay] != HappeningType.None) {
                UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.TimePanel.HappeningButton, _happeningHint,
                    delegate { KnowledgeManager.AddKnowledge(Knowledge.LilCalendar); }, true);
            }
        }

        private void ShowWeatherHint() {
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.TimePanel.CalendarPanel.transform, _weatherHint,
                delegate { KnowledgeManager.AddKnowledge(Knowledge.Weather); });
        }

        private void TryShowFoodMarketHint() {
            if (Days[SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.FoodMarket) {
                UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.ShopsPanel.BuildingShopButton, _foodMarketHint,
                    delegate { KnowledgeManager.AddKnowledge(Knowledge.FoodMarket); }, true);
                
            }
        }

        private void TryChangeMonth() {
            if (SaveLoadManager.CurrentSave.CurrentDay >= SaveLoadManager.CurrentSave.Days.Count) {
                SaveLoadManager.CurrentSave.CurrentMonth++;
                InventoryManager.Instance.ToolsActivated[ToolBuff.Weatherometr] = 0;
                GenerateDays(SaveLoadManager.CurrentSave.CurrentMonth);
            }
        }

        public IEnumerator DayPointCoroutine() {
            PlayerController.CanInteract = false;
            SaveLoadManager.CurrentSave.DayOfWeek = NextDay(SaveLoadManager.CurrentSave.DayOfWeek);
            /*if ( SaveLoadManager.CurrentSave.CurrentDay == MaxDays)
                EndMonth();*/

            SeedShopView.ChangeSeedsNewDay();

            TimePanel.UpdateLilCalendar(SaveLoadManager.CurrentSave.CurrentDay);

            InventoryManager.Instance.BrokeTools();
            UIHud.Instance.FastPanelScript.UpdateToolsImages();
            ToolShop.ChangeToolsNewDay();

            HappeningType nextDay = Days[SaveLoadManager.CurrentSave.CurrentDay];

            StartCoroutine(UIHud.screenEffect.ChangeEffectCoroutine(nextDay, false));
            yield return StartCoroutine(UIHud.screenEffect.PlayOverNightAnimation());
            if (GameModeManager.Instance.GameMode != GameMode.Training) {
                if (nextDay == HappeningType.FoodMarket) {
                    UIHud.OpenBuildingsShop();
                } else {
                    UIHud.CloseBuildingsShop();
                }
            }

            yield return StartCoroutine(SmartTilemap.NewDay(nextDay));
            UIHud.Instance.ClockView.SetInteractable(true);
            PlayerController.CanInteract = true;
        }

        public void SkipToEndMonth() {
            SaveLoadManager.CurrentSave.DayOfWeek = NextDay(LastDayInMonth(SaveLoadManager.CurrentSave.CurrentDay,
                MaxDays, SaveLoadManager.CurrentSave.DayOfWeek));
            //EndMonth();
        }
/*
        private void EndMonth() {
            InventoryManager.Instance.ToolsInventory[ToolBuff.Weatherometr] = 0;
            GenerateDays(false, false);

            if (GameModeManager.Instance.GameMode == GameMode.Training) {
                EndTrainingPanel.ShowEndPanel( SaveLoadManager.CurrentSave.CropPoints, (int) SessionTime,
                    SaveLoadManager.CurrentSave.CropsCollected);
                GpsManager.ReportScore( SaveLoadManager.CurrentSave.CropPoints, "tutorialLeaderboard");
            } else {
                EndMonthPanel.ShowEndMonthPanel( SaveLoadManager.CurrentSave.CropsCollected,
                    SaveLoadManager.CurrentSave.CropPoints);
                SaveLoadManager.CurrentSave.CropsCollected = new Queue<Crop>();
            }
        }*/

        public void ChangeDayPoint(TimeSpan timespan) { }

        private int NextDay(int currentDayOfWeek) {
            int next = (currentDayOfWeek + 1) % 7;
            return next;
        }

        private int FirstDayInMonth(int year, int month) {
            DateTime date = new(year, month, 1);
            return (int)date.DayOfWeek;
        }

        private int LastDayInMonth(int currentday, int maxDays, int currentDayofWeek) {
            int tmp = currentday % 7; // какой сейчас должен был быть день недели, если бы начиналось с понедельника
            int tmp2 = currentDayofWeek - tmp;
            int tmp3 = (maxDays % 7 + tmp2 - 1) % 7;

            return tmp3;
        }
    }
}