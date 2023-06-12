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

    public static ToolSO ToolByType(ToolBuff buff) {
        for (int i = 0; i < instance.ToolsSO.Length; i++)
            if (instance.ToolsSO[i].buff == buff)
                return instance.ToolsSO[i];
        UnityEngine.Debug.Log("Нет класса Tool под тип " + buff);
        return null;
    }
}

[Serializable]
public enum ToolBuff {
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