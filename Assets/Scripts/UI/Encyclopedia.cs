using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encyclopedia : MonoBehaviourSoundStarter
{
    public FactsPage FactsPage;
	public GameObject GridButtonPrefab;

	[Header("Crops")]
	public GameObject CropsGrid;
	public Button CropsOpenButton;
	public CropsTable CropsTablePrefab;

	[Header("Weather")]
	public Button WeatherOpenButton;
	public GameObject WeatherGrid;
	public WeatherTable WeatherTablePrefab;

	[Header("Tools")]
	public Button ToolsOpenButton;
	public GameObject ToolsGrid;
	public ToolsTable ToolsTablePrefab;

	GameObject[] cropsButtons;
	GameObject[] weatherButtons;
	GameObject[] toolButtons;

	/**********/

	private void Start()
	{
		GenerateAllButtons();
	}

	public void GenerateAllButtons()
    {
		GenerateCropsButtons();
		GenerateWeatherButtons();
		GenerateToolsButtons();

		//Кнопке открытия овощей добавляем функцию переключения на первый овощ
		CropSO crop = CropsTablePrefab.Crops[0];
		CropsOpenButton.onClick.AddListener(() => FactsPage.UpdatePage(crop.header, crop.firstText, crop.firstSprite, crop.secondText, crop.secondSprite));

		//Аналогично для кнопки отыкрытия погоды
		WeatherSO weather = WeatherTablePrefab.WeathersSO[0];
		WeatherOpenButton.onClick.AddListener(() => FactsPage.UpdatePage(weather.header, weather.firstText, weather.firstSprite, weather.secondText, weather.secondSprite));

		ToolSO tool = ToolsTablePrefab.ToolsSO[0];
		ToolsOpenButton.onClick.AddListener(() => FactsPage.UpdatePage(tool.header, tool.firstText, tool.firstSprite, tool.secondText, tool.secondSprite));

		//Открываем страничку с помидором при старте
		FactsPage.UpdatePage(crop.header, crop.firstText, crop.firstSprite, crop.secondText, crop.secondSprite);
	}

	



	public void GenerateWeatherButtons()
	{
		
		weatherButtons = new GameObject[WeatherTablePrefab.WeathersSO.Length];
		for (int i = 0; i < weatherButtons.Length; i++)
		{
			weatherButtons[i] = Instantiate(GridButtonPrefab, WeatherGrid.transform);
			weatherButtons[i].SetActive(true);

			WeatherSO weather = WeatherTablePrefab.WeathersSO[i];
			Button button = weatherButtons[i].GetComponent<Button>();
			button.onClick.AddListener(() => FactsPage.UpdatePage(weather.header, weather.firstText, weather.firstSprite, weather.secondText, weather.secondSprite));

			weatherButtons[i].GetComponent<Image>().sprite = weather.icon;
		}
		  
	}



	public void GenerateCropsButtons()
    {
		cropsButtons = new GameObject[CropsTablePrefab.Crops.Length];
			for (int i = 0; i < cropsButtons.Length; i++)
			{
			cropsButtons[i] = Instantiate(GridButtonPrefab, CropsGrid.transform);
			cropsButtons[i].SetActive(true);

			CropSO crop = CropsTablePrefab.Crops[i];
				Button button = cropsButtons[i].GetComponent<Button>();
				button.onClick.AddListener(()   => FactsPage.UpdatePage(crop.header,crop.firstText,crop.firstSprite,crop.secondText,crop.secondSprite));

				cropsButtons[i].GetComponent<Image>().sprite = crop.VegSprite;
			}
		
	}

	public void GenerateToolsButtons()
	{

		toolButtons = new GameObject[ToolsTablePrefab.ToolsSO.Length];
		for (int i = 0; i < toolButtons.Length; i++)
		{
			toolButtons[i] = Instantiate(GridButtonPrefab, ToolsGrid.transform);
			toolButtons[i].SetActive(true);

			ToolSO tool = ToolsTablePrefab.ToolsSO[i];
			Button button = toolButtons[i].GetComponent<Button>();
			button.onClick.AddListener(() => FactsPage.UpdatePage(tool.header, tool.firstText, tool.firstSprite, tool.secondText, tool.secondSprite));

			toolButtons[i].GetComponent<Image>().sprite = tool.FoodMarketSprite;
		}

	}

	public new void PlaySound(int soundIndex)
	{
		AudioManager.instance.PlaySound((Sounds)soundIndex);
	}


}
