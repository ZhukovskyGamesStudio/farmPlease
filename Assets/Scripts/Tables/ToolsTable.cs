using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolsTable : MonoBehaviour
{
    public ToolSO[] ToolsSO;
    public static ToolsTable instance;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static ToolSO ToolByType(ToolType type)
    {
        for (int i = 0; i < instance.ToolsSO.Length; i++)
        {
            if (instance.ToolsSO[i].type == type)
                return instance.ToolsSO[i];
        }
        Debug.Log("Нет класса Tool под тип " + type);
        return null;
    }
}

[System.Serializable]
public enum ToolType
{
    Doublehoe, Unlimitedwatercan, Weatherometr, Carpetseeder, Greenscythe, Wetscythe
}

[System.Serializable]
public enum ToolUIType
{
    Hoe, Watercan, Seed, Scythe, Calendar
}
