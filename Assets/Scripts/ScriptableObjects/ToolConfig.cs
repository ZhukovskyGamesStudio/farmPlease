using System;
using Tables;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Tool", menuName = "Scriptable Objects/Tool", order = 3)]
    [Serializable]
    public class ToolConfig : ConfigWithCroponomPage {
        [Header("Tool")]
        public new string name;

        public ToolBuff buff;

        [Header("ToolShopProperties")]
        public int cost;

        public string explainText;
        [Min(1)]
        public int buyAmount;
        [Min(1)]
        public int workDaysAmount = 1;
        public bool IsInstant;

        [Header("HUDElements")]
        public ToolUIType toolUIType;

        public Sprite buffedIcon;

        [Header("FoodMarketProperties")]
        public bool isAlwaysAvailable;

        public Sprite FoodMarketSprite;
        public override string GetUnlockable() => buff.ToString();
    }
}