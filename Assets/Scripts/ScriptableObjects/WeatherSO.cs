using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weather", menuName = "ScriptableObjects/Weather", order = 4)]
[Serializable]
public class WeatherSO : SOWithCroponomPage {
    [Header("Weather")]
    public HappeningType type;

    public Sprite icon;

    [Header("HUDElements")]
    public Sprite DaySprite;
}