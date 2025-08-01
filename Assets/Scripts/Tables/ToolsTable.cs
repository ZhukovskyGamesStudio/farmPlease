﻿using System;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using ScriptableObjects;

namespace Tables
{
    public class ToolsTable : PreloadableSingleton<ToolsTable> {
        public ToolConfig[] ToolsSO;

        public override int InitPriority => -10000;

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
        None = -1,
        Doublehoe,
        Unlimitedwatercan,
        Weatherometr,
        Carpetseeder,
        Greenscythe,
        Wetscythe,
        WeekBattery
    }
    [Serializable]
    public enum ToolBuffRussion {
        Ничто = -1,
        Двойная_тяпка,
        Бездонная_лейка,
        Погодомер,
        Подшитый_рюкзак,
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