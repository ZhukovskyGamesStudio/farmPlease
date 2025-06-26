using System;
using Abstract;
using ScriptableObjects;

namespace Tables {
    public class WeatherTable : PreloadableSingleton<WeatherTable> {
        public WeatherConfig[] WeathersSO;
        public override int InitPriority => -10000;
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
        NormalSunnyDay = 0,
        FoodMarket,
        Unknown,
        Rain,
        Erosion,
        Wind,
        Insects,
        Love
    }
}