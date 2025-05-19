using System;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using ScriptableObjects;
using UnityEngine;

namespace Tables
{
    public class ToolsTable : PreloadableSingleton<ToolsTable> {
        public ToolConfig[] ToolsSO;

        public static ToolConfig ToolByType(ToolBuff buff) {
            for (int i = 0; i < Instance.ToolsSO.Length; i++)
                if (Instance.ToolsSO[i].buff == buff)
                    return Instance.ToolsSO[i];
            UnityEngine.Debug.Log("Нет класса Tool под тип " + buff);
            return null;
        }

        public static List<ToolBuff> Tools=> Instance.ToolsSO.Select(t => t.buff).ToList();
    }

    [Serializable]
    public enum ToolBuff {
        Doublehoe,
        Unlimitedwatercan,
        Weatherometr,
        Carpetseeder,
        Greenscythe,
        Wetscythe,
        WeekBattery
    }

    [Serializable]
    public enum ToolUIType {
        Hoe,
        Watercan,
        Seed,
        Scythe,
        Calendar,
        None
    }
}