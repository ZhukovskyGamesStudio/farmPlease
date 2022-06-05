using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimePanel : MonoBehaviour
{
	int DaysAmount;

	int SkipDaysAmount;

	public DayScript lilDay;
	public Image CalendarImage;

	public GameObject CalendarPanel;
	public GameObject DayPref;
	public GameObject DaysParent;
	public GameObject timeHintPanel;
	public Text TimeOfDayText;
	GameObject[] Days;
	GameObject[] SkipDays;
	public bool isOpen;
	HappeningType[] daysHappenings;

	public void CreateDays(HappeningType[] daysHappenings, int skipAmount)
	{
		this.daysHappenings = daysHappenings;
		DaysAmount = daysHappenings.Length;
		SkipDaysAmount = skipAmount;

		if (SkipDays != null)
			for (int i = 0; i < SkipDays.Length; i++)
				Destroy(SkipDays[i]);


		if (SkipDaysAmount > 0)
		{
			SkipDays = new GameObject[SkipDaysAmount];
			for (int i = 0; i < SkipDaysAmount; i++)
			{
				SkipDays[i] = Instantiate(DayPref, DaysParent.transform);
				SkipDays[i].GetComponent<DayScript>().Clear();
			}
		}


		if (Days != null)
			for (int i = 0; i < Days.Length; i++)
				Destroy(Days[i]);


		Days = new GameObject[DaysAmount];
		for (int i = 0; i < DaysAmount; i++)
		{
			Days[i] = Instantiate(DayPref, DaysParent.transform);
			Days[i].GetComponent<DayScript>().SetProps(i, daysHappenings[i]);
		}
	}

	public void UpdateBigCalendar(int curDay)
	{
		for (int i = 0; i < Days.Length; i++)
		{
			DayScript script = Days[i].GetComponent<DayScript>();
			if (daysHappenings[i] == HappeningType.Love && !InventoryManager.instance.IsToolWorking(ToolType.Weatherometr))
				script.SetProps(i, HappeningType.None);
			else if (daysHappenings[i] != HappeningType.None && daysHappenings[i] != HappeningType.Marketplace && !InventoryManager.instance.IsToolWorking(ToolType.Weatherometr))
				script.SetProps(i, HappeningType.Unknown);
			else
				script.SetProps(i, daysHappenings[i]);

			if (i < curDay)
				script.DayOver();

		}
	}

	public void AddDayButton()
    {
		if (GameModeManager.instance.GameMode == GameMode.Training)
			TimeManager.instance.AddDay();
		else
        {
			timeHintPanel.SetActive(true);
		}
	}


	public void UpdateLilCalendar(int date)
	{
		
		
		if (daysHappenings[date] != HappeningType.None && daysHappenings[date] != HappeningType.Marketplace && !InventoryManager.instance.IsToolWorking(ToolType.Weatherometr))
			lilDay.SetProps(date, HappeningType.Unknown);
		else
			lilDay.SetProps(date, daysHappenings[date], true);
	}

	public void CalendarPanelOpenClose()
    {
		isOpen = !isOpen;
		CalendarPanel.SetActive(isOpen);
	}
}
