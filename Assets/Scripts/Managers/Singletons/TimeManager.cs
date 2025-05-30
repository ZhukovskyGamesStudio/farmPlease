using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using Tables;
using UI;
using UnityEngine;
using ZhukovskyGamesPlugin;
using Random = UnityEngine.Random;

namespace Managers {
    public class TimeManager : Singleton<TimeManager> {
        public static DateTime FirstDayOfGame => new DateTime(2051, 5, 1);

        [Range(1, 31)]
        public static int MaxDays;
        
        private static int _skipDaysAmount;

        [SerializeField]
        private SpotlightAnimConfig _calendarHint, _weatherHint, _happeningHint,_toolMarketHint, _foodMarketHint;

        public static List<HappeningType> Days => SaveLoadManager.CurrentSave.Days;

        public float SessionTime;

        private PlayerController PlayerController => global::PlayerController.Instance;
        private SeedShopView SeedShopView => UIHud.ShopsPanel.seedShopView;
        private SmartTilemap SmartTilemap => SmartTilemap.Instance;
        private TimePanelView TimePanelView => UIHud.TimePanelView;
        private ToolShopView ToolShop => UIHud.ShopsPanel.toolShopView;

        private UIHud UIHud => global::UI.UIHud.Instance;

        protected override void OnFirstInit() {
            SessionTime = 0;
        }

        private void Update() {
            SessionTime += UnityEngine.Time.deltaTime;
        }

        public bool IsTodayLoveDay => Days[SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.Love;

        public void SetDaysWithData(List<HappeningType> daysData, DateTime date) {

            if (daysData == null) {
                GenerateDays(0);
                return;
            }

            TryChangeMonth();

            MaxDays = daysData.Count;
            _skipDaysAmount = FirstDayInMonth(date.Year, date.Month);

            TimePanelView.gameObject.SetActive(SaveLoadManager.CurrentSave.KnowledgeList.Contains(Knowledge.Weather));
            TimePanelView.CreateDays(Days, _skipDaysAmount);
            TimePanelView.UpdateLilCalendar(SaveLoadManager.CurrentSave.CurrentDay);
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

        public static void GenerateDays(int month) {
            DateTime date = FirstDayOfGame.AddMonths(month);
            MaxDays = DateTime.DaysInMonth(date.Year, date.Month);
            _skipDaysAmount = FirstDayInMonth(date.Year, date.Month);

            SaveLoadManager.CurrentSave.CurrentDay = 0;

            GenerateHappenings();

            //TimePanelView.CreateDays(Days, SkipDaysAmount);

            //TimePanel.UpdateLilCalendar(SaveLoadManager.CurrentSave.CurrentDay);
            //ToolShop.ChangeToolsNewDay();

           /* if (GameModeManager.Instance.GameMode != GameMode.Training) {
                if (Days[SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.FoodMarket) {
                    UIHud.OpenBuildingsShop();
                } else {
                    UIHud.CloseBuildingsShop();
                }
            }*/
        }

        private static void GenerateHappenings() {
            SaveLoadManager.CurrentSave.Days = new List<HappeningType>();
            for (int i = 0; i < MaxDays; i++) {
                Days.Add(HappeningType.None);
            }

            int love = -1;
            bool isTrainingRainNext = SaveLoadManager.CurrentSave.CurrentMonth == 0 && SaveLoadManager.CurrentSave.CurrentDay == 0;
            while ((love + _skipDaysAmount) % 7 == 0 || (love + 1) % 5 == 0) love = Random.Range(7 + _skipDaysAmount, 20);

            for (int i = 0; i < MaxDays; i++) {
                int x = i + _skipDaysAmount;
                if (x % 7 == 0 && x > 0 && GameModeManager.Instance.GameMode != GameMode.Training && SaveLoadManager.CurrentSave.CurrentMonth > 0) {
                    Days[i] = HappeningType.FoodMarket;
                } else if ((i + 1) % 5 == 0) {
                    if (isTrainingRainNext) {
                        Days[i] = HappeningType.Rain;
                        isTrainingRainNext = false;
                        continue;
                    }

                  
                    List<HappeningType> possibleHappenings = new List<HappeningType> {
                        HappeningType.Rain, HappeningType.Erosion, HappeningType.Wind, HappeningType.Insects
                    };
                    if (UnlockableUtils.HasUnlockable(HappeningType.Rain)) {
                        possibleHappenings.Add(HappeningType.Rain);
                    }
                    if (UnlockableUtils.HasUnlockable(HappeningType.Erosion)) {
                        possibleHappenings.Add(HappeningType.Erosion);
                    }
                    if (UnlockableUtils.HasUnlockable(HappeningType.Wind)) {
                        possibleHappenings.Add(HappeningType.Wind);
                    }
                    if (UnlockableUtils.HasUnlockable(HappeningType.Insects)) {
                        possibleHappenings.Add(HappeningType.Insects);
                    }
                    int rnd = Random.Range(0, possibleHappenings.Count);
                    Days[i] = possibleHappenings[rnd];
                } else if (i == love &&  UnlockableUtils.HasUnlockable(HappeningType.Love)) {
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
                UIHud.TimePanelView.gameObject.SetActive(true);
                UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.TimePanelView.CalendarButton, _calendarHint, ShowWeatherHint);
            }
        }

        private void TryShowHappeningHint() {
            if (Days[SaveLoadManager.CurrentSave.CurrentDay] != HappeningType.None) {
                UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.TimePanelView.HappeningButton, _happeningHint,
                    delegate { KnowledgeUtils.AddKnowledge(Knowledge.LilCalendar); }, true);
            }
        }

        private void ShowWeatherHint() {
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.TimePanelView.CalendarPanel.transform, _weatherHint,
                delegate { KnowledgeUtils.AddKnowledge(Knowledge.Weather); });
        }
        
        
       

        private void TryShowFoodMarketHint() {
            if (Days[SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.FoodMarket) {
               UnlockableUtils.Unlock(HappeningType.FoodMarket);
            }
        }

        private void TryChangeMonth() {
            if (SaveLoadManager.CurrentSave.CurrentDay >= SaveLoadManager.CurrentSave.Days.Count) {
                SaveLoadManager.CurrentSave.CurrentMonth++;
                InventoryManager.ToolsActivated[ToolBuff.Weatherometr] = 0;
                GenerateDays(SaveLoadManager.CurrentSave.CurrentMonth);
            }
        }

        public IEnumerator DayPointCoroutine() {
            PlayerController.CanInteract = false;
            SaveLoadManager.CurrentSave.DayOfWeek = NextDay(SaveLoadManager.CurrentSave.DayOfWeek);
            /*if ( SaveLoadManager.CurrentSave.CurrentDay == MaxDays)
                EndMonth();*/

            SeedShopView.ChangeSeedsNewDay();

            TimePanelView.UpdateLilCalendar(SaveLoadManager.CurrentSave.CurrentDay);

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


        private int NextDay(int currentDayOfWeek) {
            int next = (currentDayOfWeek + 1) % 7;
            return next;
        }

        private static int FirstDayInMonth(int year, int month) {
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