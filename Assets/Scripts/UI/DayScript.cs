using UnityEngine;
using UnityEngine.UI;

public class DayScript : MonoBehaviour
{
	public Image Date;
	public Image Happening;
	public GameObject Finished;

	public Sprite[] DateSprites;

	public void Clear()
	{
		Date.gameObject.SetActive(false);
		Happening.gameObject.SetActive(false);
		Finished.gameObject.SetActive(false);
	}


	public void DayOver()
	{
		Finished.SetActive(true);
		Date.color = new Color(1, 1, 1, 0.5f);
		Happening.color = new Color(1, 1, 1, 0.5f);
	}

	public void SetProps(int dayNumber, HappeningType type,bool showDefault = false)
	{
		if (dayNumber == -1)
		{
			Clear(); return;
		}
		else
		{
			Date.sprite = DateSprites[dayNumber];
			SetHappening(type, showDefault);
		}
	}

	public void SetProps(DayScript dayScript)
	{
		Date.sprite = dayScript.Date.sprite;
		Happening.sprite = dayScript.Happening.sprite;

	}


	public void SetHappening(HappeningType type, bool showDefault = false)
	{
		Happening.gameObject.SetActive(true);
		Happening.sprite = WeatherTable.WeatherByType(type).DaySprite;

		if (type == HappeningType.None && !showDefault)
		{
			Happening.gameObject.SetActive(false);
			return;
		}
		else
		{
			Happening.gameObject.SetActive(true);
			Happening.sprite = WeatherTable.WeatherByType(type).DaySprite;
		}  
	}

}
