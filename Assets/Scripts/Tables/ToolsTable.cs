using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace Tables
{
    public class ToolsTable : MonoBehaviour {
        public static ToolsTable Instance;
        public ToolConfig[] ToolsSO;

        public void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

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
}