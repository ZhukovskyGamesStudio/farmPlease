using System;
using UnityEngine;

public class ToolsTable : MonoBehaviour {
    public static ToolsTable instance;
    public ToolSO[] ToolsSO;

    public void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static ToolSO ToolByType(ToolType type) {
        for (int i = 0; i < instance.ToolsSO.Length; i++)
            if (instance.ToolsSO[i].type == type)
                return instance.ToolsSO[i];
        Debug.Log("Нет класса Tool под тип " + type);
        return null;
    }
}

[Serializable]
public enum ToolType {
    Doublehoe,
    Unlimitedwatercan,
    Weatherometr,
    Carpetseeder,
    Greenscythe,
    Wetscythe
}

[Serializable]
public enum ToolUIType {
    Hoe,
    Watercan,
    Seed,
    Scythe,
    Calendar
}