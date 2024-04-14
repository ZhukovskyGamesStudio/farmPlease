using System;
using ScriptableObjects;
using UnityEngine;

namespace Tables
{
    public class WeatherTable : MonoBehaviour {
        public static WeatherTable Instance;
        public WeatherConfig[] WeathersSO;

        public void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public static WeatherConfig WeatherByType(HappeningType type) {
            for (int i = 0; i < Instance.WeathersSO.Length; i++)
                if (Instance.WeathersSO[i].type == type)
                    return Instance.WeathersSO[i];
            UnityEngine.Debug.Log("Нет класса Weather под тип " + type);
            return null;
        }
    }

    [Serializable]
    public enum HappeningType {
        None = 0,
        FoodMarket,
        Unknown,
        Rain,
        Erosion,
        Wind,
        Insects,
        Love
    }
}