using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weather", menuName = "ScriptableObjects/Weather", order = 4)]
[Serializable]
public class WeatherConfig : ConfigWithCroponomPage {
    [Header("Weather")]
    public HappeningType type;

    public Sprite icon;

    [Header("HUDElements")]
    public Sprite DaySprite;
}