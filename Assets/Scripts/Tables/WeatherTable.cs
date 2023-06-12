using System;
using UnityEngine;

public class WeatherTable : MonoBehaviour {
    public static WeatherTable instance;
    public WeatherSO[] WeathersSO;

    public void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static WeatherSO WeatherByType(HappeningType type) {
        for (int i = 0; i < instance.WeathersSO.Length; i++)
            if (instance.WeathersSO[i].type == type)
                return instance.WeathersSO[i];
        UnityEngine.Debug.Log("Нет класса Weather под тип " + type);
        return null;
    }
}

[Serializable]
public enum HappeningType {
    None = 0,
    Marketplace,
    Unknown,
    Rain,
    Erosion,
    Wind,
    Insects,
    Love
}