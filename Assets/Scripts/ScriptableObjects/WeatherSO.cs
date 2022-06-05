using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weather", menuName = "ScriptableObjects/Weather", order = 4)]
[System.Serializable]
public class WeatherSO : ScriptableObject
{
    public HappeningType type;
    public Sprite icon;

    [Header("HUDElements")]
    public Sprite DaySprite;


    [Header("CroponomPage")]
    public string header;
    public string firstText;
    public Sprite firstSprite;
    public string secondText;
    public Sprite secondSprite;

}