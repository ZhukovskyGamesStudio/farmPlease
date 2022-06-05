using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherTable : MonoBehaviour
{
    public WeatherSO[] WeathersSO;

    public static WeatherTable instance;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static WeatherSO WeatherByType(HappeningType type)
    {
        for (int i = 0; i < instance.WeathersSO.Length; i++)
        {
            if (instance.WeathersSO[i].type == type)
                return instance.WeathersSO[i];
        }
        Debug.Log("Нет класса Weather под тип " + type);
        return null;
    }
}

public enum HappeningType
{
    None = 0, Marketplace, Unknown, Rain, Erosion, Wind, Insects, Love
}