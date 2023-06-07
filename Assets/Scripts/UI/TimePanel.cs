using DefaultNamespace.Managers;
using UnityEngine;
using UnityEngine.UI;

public class TimePanel : MonoBehaviour {
    public DayScript lilDay;
    public Image CalendarImage;

    public GameObject CalendarPanel;
    public GameObject DayPref;
    public GameObject DaysParent;
    public GameObject timeHintPanel;
    public Text TimeOfDayText;
    public bool isOpen;
    private GameObject[] Days;
    private int DaysAmount;
    private HappeningType[] daysHappenings;
    private GameObject[] SkipDays;

    private int SkipDaysAmount;

    public void CreateDays(HappeningType[] daysHappenings, int skipAmount) {
        this.daysHappenings = daysHappenings;
        DaysAmount = daysHappenings.Length;
        SkipDaysAmount = skipAmount;

        if (SkipDays != null)
            for (int i = 0; i < SkipDays.Length; i++)
                Destroy(SkipDays[i]);

        if (SkipDaysAmount > 0) {
            SkipDays = new GameObject[SkipDaysAmount];
            for (int i = 0; i < SkipDaysAmount; i++) {
                SkipDays[i] = Instantiate(DayPref, DaysParent.transform);
                SkipDays[i].GetComponent<DayScript>().Clear();
            }
        }

        if (Days != null)
            for (int i = 0; i < Days.Length; i++)
                Destroy(Days[i]);

        Days = new GameObject[DaysAmount];
        for (int i = 0; i < DaysAmount; i++) {
            Days[i] = Instantiate(DayPref, DaysParent.transform);
            Days[i].GetComponent<DayScript>().SetProps(i, daysHappenings[i]);
        }
    }

    private void UpdateBigCalendar(int curDay) {
        for (int i = 0; i < Days.Length; i++) {
            DayScript script = Days[i].GetComponent<DayScript>();
            if (daysHappenings[i] == HappeningType.Love &&
                !InventoryManager.instance.IsToolWorking(ToolType.Weatherometr))
                script.SetProps(i, HappeningType.None);
            else if (daysHappenings[i] != HappeningType.None && daysHappenings[i] != HappeningType.Marketplace &&
                     !InventoryManager.instance.IsToolWorking(ToolType.Weatherometr))
                script.SetProps(i, HappeningType.Unknown);
            else
                script.SetProps(i, daysHappenings[i]);

            if (i < curDay)
                script.DayOver();
        }
    }

    public void UpdateLilCalendar(int date) {
        if (daysHappenings[date] != HappeningType.None && daysHappenings[date] != HappeningType.Marketplace &&
            !InventoryManager.instance.IsToolWorking(ToolType.Weatherometr))
            lilDay.SetProps(date, HappeningType.Unknown);
        else
            lilDay.SetProps(date, daysHappenings[date], true);
    }

    public void CalendarPanelOpenClose() {
        isOpen = !isOpen;
        CalendarPanel.SetActive(isOpen);
        if (isOpen) {
            UpdateBigCalendar(Time.Instance.day);
        }
    }
}