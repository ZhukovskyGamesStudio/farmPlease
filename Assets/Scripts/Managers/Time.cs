using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Abstract;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Time : Singleton<Time> {
    public int time;

    [Range(1, 31)]
    public int MaxDays;

    [Range(0, 7)]
    public int SkipDaysAmount;

    public EndTrainingPanel EndTrainingPanel;
    public EndMonthPanel EndMonthPanel;
    public List<HappeningType> Days => SaveLoadManager.CurrentSave.Days;

    public float SessionTime;
    public bool isLoaded;

    private FastPanelScript FastPanel  => PlayerController.GetComponent<FastPanelScript>();
    private bool IsTimerWorking;
    private PlayerController PlayerController => PlayerController.Instance;
    private SeedShopView SeedShopView => _uiHud.ShopsPanel.seedShopView;
    private SmartTilemap SmartTilemap => SmartTilemap.instance;
    private Text TimeOfDayText => TimePanel.TimeOfDayText;
    private TimePanel TimePanel => _uiHud.TimePanel;
    public DateTime tmpDate;
    private ToolShopView ToolShop => _uiHud.ShopsPanel.toolShopView;

    private UIHud _uiHud => UIHud.Instance;
    
    /**********/
    protected override void OnFirstInit() {
        isLoaded = false;
        IsTimerWorking = false;
        SessionTime = 0;
    }

    private void Update() {
        SessionTime += UnityEngine.Time.deltaTime;
    }

    /**********/

    public bool IsTodayLoveDay => Days[SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.Love;

    public void CalculateTimeSpan(DateTime tmpDate) {
        StartCoroutine(CalculateTimeSpanCoroutine(tmpDate));
    }

    public IEnumerator CalculateTimeSpanCoroutine(DateTime tmpDate) {
        int daysGone = RealTImeManager.DaysGone(tmpDate);

        if (RealTImeManager.IsRefillingEnergy(tmpDate)) {
            Energy.Instance.RestoreEnergy();
        }

        if (daysGone > 0) {
            SaveLoadManager.Instance.Sequence(true);

            for (int i = 0; i < daysGone; i++)
                yield return DayPointCoroutine();

            SaveLoadManager.Instance.Sequence(false);
        }
    }

    public void SetDaysWithData(List<HappeningType> daysData, DateTime date) {
        ChangeDayPoint(Settings.Instance.GetDayPoint());

        if (daysData == null) {
            GenerateDays(GameModeManager.Instance.GameMode == GameMode.Training, false);
            return;
        }

        MaxDays = daysData.Count;
        SaveLoadManager.CurrentSave.CurrentDay = date.Day - 1;
        SaveLoadManager.CurrentSave.DayOfWeek = (int) date.DayOfWeek - 1;
        SkipDaysAmount = FirstDayInMonth(date.Year, date.Month);

        TimePanel.CreateDays(Days, SkipDaysAmount);
        TimePanel.UpdateLilCalendar( SaveLoadManager.CurrentSave.CurrentDay);

        if (GameModeManager.Instance.GameMode != GameMode.Training) {
            if (Days[ SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.Marketplace) {
                _uiHud.OpenBuildingsShop();
            } else {
                _uiHud.CloseBuildingsShop();
            }
        }

        tmpDate = date;
        isLoaded = true;
    }

    public void GenerateDays(bool isTraining, bool isNewGame) {
        if (isTraining) {
            MaxDays = 31;
            SkipDaysAmount = 0;
        } else {
            DateTime date = DateTime.Now;
            MaxDays = DateTime.DaysInMonth(date.Year, date.Month);
            SkipDaysAmount = FirstDayInMonth(date.Year, date.Month);
            ChangeDayPoint(Settings.Instance.GetDayPoint());
        }

        SaveLoadManager.CurrentSave.CurrentDay = 0;
        if (isNewGame && !isTraining)
            SaveLoadManager.CurrentSave.CurrentDay = DateTime.Now.Day - 1;
        SaveLoadManager.CurrentSave.Days = new List<HappeningType>();
        for (int i = 0; i < MaxDays; i++) {
            Days.Add(HappeningType.None);
        }
        
        int love = -1;
        while ((love + SkipDaysAmount + 1) % 7 == 0 || (love + 1) % 5 == 0) love = Random.Range(8 + SkipDaysAmount, 20);

        for (int i = 0; i < MaxDays; i++) {
            int x = i + SkipDaysAmount + 1;
            if (x % 7 == 0 && x > 0 && GameModeManager.Instance.GameMode != GameMode.Training) {
                Days[i] = HappeningType.Marketplace;
            } else if ((i + 1) % 5 == 0) {
                int rnd = Random.Range(0, 4);
                if (GameModeManager.Instance.GameMode == GameMode.Training)
                    rnd = Random.Range(0, 2);

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

        TimePanel.CreateDays(Days, SkipDaysAmount);

        TimePanel.UpdateLilCalendar( SaveLoadManager.CurrentSave.CurrentDay);
        ToolShop.ChangeTools();

        if (GameModeManager.Instance.GameMode != GameMode.Training) {
            if (Days[ SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.Marketplace) {
                _uiHud.OpenBuildingsShop();
            } else {
                _uiHud.CloseBuildingsShop();
            }
        }

        isLoaded = true;
        tmpDate = DateTime.Now;
    }

    /***********/

    public void AddDay() {
        StartCoroutine(DayPointCoroutine());
        Energy.Instance.RestoreEnergy();
    }

    public IEnumerator DayPointCoroutine() {
        SaveLoadManager.CurrentSave.CurrentDay++;

        SaveLoadManager.CurrentSave.DayOfWeek = NextDay(SaveLoadManager.CurrentSave.DayOfWeek);
        if ( SaveLoadManager.CurrentSave.CurrentDay == MaxDays)
            EndMonth();

        SeedShopView.ChangeSeedsNewDay();

        TimePanel.UpdateLilCalendar( SaveLoadManager.CurrentSave.CurrentDay);

        InventoryManager.instance.BrokeTools();
        FastPanel.UpdateToolsImages();
        ToolShop.ChangeTools();

        if (GameModeManager.Instance.GameMode != GameMode.Training) {
            if (Days[ SaveLoadManager.CurrentSave.CurrentDay] == HappeningType.Marketplace) {
                _uiHud.OpenBuildingsShop();
            } else {
                _uiHud.CloseBuildingsShop();
            }
        }

        if ( SaveLoadManager.CurrentSave.CurrentDay > 0)
            yield return StartCoroutine(SmartTilemap.EndDayEvent(Days[ SaveLoadManager.CurrentSave.CurrentDay - 1]));
        yield return StartCoroutine(SmartTilemap.NewDay());
    }

    public void SkipToEndMonth() {
        SaveLoadManager.CurrentSave.DayOfWeek = NextDay(LastDayInMonth( SaveLoadManager.CurrentSave.CurrentDay, MaxDays, SaveLoadManager.CurrentSave.DayOfWeek));
        EndMonth();
    }

    private void EndMonth() {
        InventoryManager.instance.toolsInventory[ToolBuff.Weatherometr] = 0;
        GenerateDays(false, false);

        if (GameModeManager.Instance.GameMode == GameMode.Training) {
            EndTrainingPanel.ShowEndPanel( SaveLoadManager.CurrentSave.CropPoints, (int) SessionTime,
                SaveLoadManager.CurrentSave.CropsCollected);
            Gps.ReportScore( SaveLoadManager.CurrentSave.CropPoints, "tutorialLeaderboard");
        } else {
            EndMonthPanel.ShowEndMonthPanel( SaveLoadManager.CurrentSave.CropsCollected,
                SaveLoadManager.CurrentSave.CropPoints);
            SaveLoadManager.CurrentSave.CropsCollected = new Queue<CropsType>();
        }
    }

    public void ChangeDayPoint(TimeSpan timespan) {
    }

    private int NextDay(int currentDayOfWeek) {
        int next = (currentDayOfWeek + 1) % 7;
        return next;
    }

    private int FirstDayInMonth(int year, int month) {
        DateTime date = new(year, month, 1);
        return (int) date.DayOfWeek - 1;
    }

    private int LastDayInMonth(int currentday, int maxDays, int currentDayofWeek) {
        int tmp = currentday % 7; // какой сейчас должен был быть день недели, если бы начиналось с понедельника
        int tmp2 = currentDayofWeek - tmp;
        int tmp3 = (maxDays % 7 + tmp2 - 1) % 7;

        return tmp3;
    }
}