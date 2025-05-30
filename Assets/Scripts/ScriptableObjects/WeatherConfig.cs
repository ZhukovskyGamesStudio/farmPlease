using System;
using Tables;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "Weather", menuName = "Scriptable Objects/Weather", order = 4)]
    [Serializable]
    public class WeatherConfig : ConfigWithCroponomPage {
        [Header("Weather")]
        public HappeningType type;

        public Sprite icon;

        [Header("HUDElements")]
        public Sprite DaySprite;

        public override string GetUnlockable() => type.ToString();
    }
}