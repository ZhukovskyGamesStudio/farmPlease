﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        private SpotlightAnimConfig _calendarHint, _weatherHint, _happeningHint, _toolMarketHint, _foodMarketHint;

        public static List<HappeningType> Days => SaveLoadManager.CurrentSave.Days;

        public float SessionTime;

        private PlayerController PlayerController => global::PlayerController.Instance;
        private SmartTilemap SmartTilemap => SmartTilemap.Instance;
        private TimePanel TimePanel => UIHud.TimePanel;

        private UIHud UIHud => global::UI.UIHud.Instance;

        protected override void OnFirstInit() {
            SessionTime = 0;
        }

        private void Update() {
            SessionTime += UnityEngine.Time.deltaTime;
        }

        public bool IsTodayLoveDay => Days[SaveLoadManager.CurrentSave.CurrentDayInMonth] == HappeningType.Love;

        public void SetDaysWithData(List<HappeningType> daysData, DateTime date) {
            if (daysData == null) {
                GenerateDays(date);
                return;
            }

            MaxDays = daysData.Count;
            _skipDaysAmount = FirstDayInMonth(date.Year, date.Month);

            TimePanel.gameObject.SetActive(SaveLoadManager.CurrentSave.KnowledgeList.Contains(Knowledge.Weather));

            TimePanel.UpdateLilCalendar(SaveLoadManager.CurrentSave.CurrentDayInMonth);
            SmartTilemap.SetHappeningType(Days[SaveLoadManager.CurrentSave.CurrentDayInMonth]);
            if (GameModeManager.Instance.GameMode != GameMode.Training) {
                if (Days[SaveLoadManager.CurrentSave.CurrentDayInMonth] == HappeningType.FoodMarket) {
                    UIHud.OpenBuildingsShop();
                } else {
                    UIHud.CloseBuildingsShop();
                }
            }

            StartCoroutine(UIHud.screenEffect.SetEffectCoroutine(Days[SaveLoadManager.CurrentSave.CurrentDayInMonth], false));
        }

        public static void GenerateDays(DateTime date) {
            MaxDays = DateTime.DaysInMonth(date.Year, date.Month);
            _skipDaysAmount = FirstDayInMonth(date.Year, date.Month);

            SaveLoadManager.CurrentSave.CurrentDayInMonth = 0;

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
                Days.Add(HappeningType.NormalSunnyDay);
            }

            int love = -1;
            while ((love + _skipDaysAmount) % 7 == 0 || (love + 1) % 5 == 0) love = Random.Range(7 + _skipDaysAmount, 20);
            int skippedRainsNeeded = 2;

            for (int i = 0; i < MaxDays; i++) {
                int x = i + _skipDaysAmount;
                if (x % 7 == 0 && x > 0 && SaveLoadManager.CurrentSave.CurrentMonth > 0 &&
                    UnlockableUtils.HasUnlockable(HappeningType.FoodMarket)) {
                    Days[i] = HappeningType.FoodMarket;
                } else if ((i + 1) % 5 == 0) {
                    if (SaveLoadManager.CurrentSave.CurrentMonth == 0 && skippedRainsNeeded > 0) {
                        skippedRainsNeeded--;
                        continue;
                    }

                    Days[i] = HappeningType.Unknown;
                } else if (i == love && UnlockableUtils.HasUnlockable(HappeningType.Love)) {
                    Days[i] = HappeningType.Love;
                }
            }
        }

        public void AddDay() {
            SaveLoadManager.CurrentSave.CurrentDayInMonth++;
            int daysInMonth = DateTime.DaysInMonth(SaveLoadManager.CurrentSave.ParsedDate.Year, SaveLoadManager.CurrentSave.ParsedDate.Month);
            SaveLoadManager.CurrentSave.Date = SaveLoadManager.CurrentSave.ParsedDate.AddDays(1).ToString(CultureInfo.InvariantCulture);

            if (SaveLoadManager.CurrentSave.CurrentDayInMonth > daysInMonth - 1) {
                ChangeMonth();
            }

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
            int nextDay = SaveLoadManager.CurrentSave.CurrentDayInMonth + 1;
            if (Days.Count > nextDay && Days[nextDay] != HappeningType.NormalSunnyDay) {
                UIHud.TimePanel.gameObject.SetActive(true);
                UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.TimePanel.CalendarButton, _calendarHint, ShowWeatherHint);
            }
        }

        private void TryShowHappeningHint() {
            if (Days[SaveLoadManager.CurrentSave.CurrentDayInMonth] != HappeningType.NormalSunnyDay) {
                UIHud.Instance.SpotlightWithText.ShowSpotlightOnButton(UIHud.TimePanel.HappeningButton, _happeningHint,
                    delegate { KnowledgeUtils.AddKnowledge(Knowledge.LilCalendar); }, true);
            }
        }

        private void ShowWeatherHint() {
            BigCalendarDialog bigCalendarDialog = FindAnyObjectByType<BigCalendarDialog>();
            UIHud.Instance.SpotlightWithText.ShowSpotlight(bigCalendarDialog.transform, _weatherHint,
                delegate { KnowledgeUtils.AddKnowledge(Knowledge.Weather); });
        }

        private void TryShowFoodMarketHint() {
            if (Days[SaveLoadManager.CurrentSave.CurrentDayInMonth] == HappeningType.FoodMarket) {
                UnlockableUtils.Unlock(HappeningType.FoodMarket);
            }
        }

        private void ChangeMonth() {
            SaveLoadManager.CurrentSave.CurrentMonth++;
            GenerateDays(SaveLoadManager.CurrentSave.ParsedDate);
        }

        public IEnumerator DayPointCoroutine() {
            PlayerController.CanInteract = false;
            SaveLoadManager.CurrentSave.DayOfWeek = NextDay(SaveLoadManager.CurrentSave.DayOfWeek);
            /*if ( SaveLoadManager.CurrentSave.CurrentDay == MaxDays)
                EndMonth();*/

            InventoryManager.Instance.BrokeTools();
            UIHud.Instance.FastPanelScript.UpdateToolsImages();
            ChangeSeedsNewDay();
            ChangeToolsNewDay();

            HappeningType nextDay = UnveilUnknownHappening(SaveLoadManager.CurrentSave.CurrentDayInMonth);
            TimePanel.UpdateLilCalendar(SaveLoadManager.CurrentSave.CurrentDayInMonth);
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

        private void ChangeSeedsNewDay() {
            SaveLoadManager.CurrentSave.SeedShopData.ChangeButtonActive = true;
            SaveLoadManager.CurrentSave.SeedShopData.NeedShowChange = true;
            SeedsUtils.ChangeSeeds();
        }
        
        private void ChangeToolsNewDay() {
            SaveLoadManager.CurrentSave.ToolShopData.ChangeButtonActive = true;
            ToolsUtils.ChangeTools();
        }

        public static HappeningType UnveilUnknownHappening(int day) {
            HappeningType happening = Days[day];
            if (happening == HappeningType.Unknown) {
                List<HappeningType> possibleHappenings = new List<HappeningType>();
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
                Days[day] = possibleHappenings[rnd];
                happening = possibleHappenings[rnd];
            }

            return happening;
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

        public static int FirstDayInMonth(int year, int month) {
            DateTime date = new(year, month, 1);
            return (int)date.DayOfWeek;
        }

        private int LastDayInMonth(int currentday, int maxDays, int currentDayofWeek) {
            int tmp = currentday % 7; // какой сейчас должен был быть день недели, если бы начиналось с понедельника
            int tmp2 = currentDayofWeek - tmp;
            int tmp3 = (maxDays % 7 + tmp2 - 1) % 7;

            return tmp3;
        }

        public void ShowBigCalendarDialog() {
            var date = SaveLoadManager.CurrentSave.ParsedDate;
            var skipDaysAmount = FirstDayInMonth(date.Year, date.Month);
            DialogsManager.Instance.ShowDialogWithData(typeof(BigCalendarDialog), new BigCalendarData() {
                DaysHappenings = Days,
                SkipAmount = skipDaysAmount
            });
        }
    }
}