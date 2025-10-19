using System;
using Tables;
using UnityEngine;
using ZG_Localization;
namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Tool", menuName = "Scriptable Objects/Tool", order = 3)]
    [Serializable]
    public class ToolConfig : ConfigWithCroponomPage {
        [Header("Tool")]
        public ToolBuff buff;

        [Header("ToolShopProperties")]
        public int cost;

        [LocalizationKey("Croponom")] 
        public string explainTextLoc;
        [Min(1)]
        public int buyAmount;
        [Min(1)]
        public int workDaysAmount = 1;
        public bool IsInstant;
        public bool IsInstantUse = true;

        [Header("HUDElements")]
        public ToolUIType toolUIType;

        public Sprite buffedIcon;

        [Header("FoodMarketProperties")]
        public bool isAlwaysAvailable;

        public Sprite FoodMarketSprite;
        public override string GetUnlockable() => buff.ToString();
        public override int GetPageIndex() {
            return (int)buff + 200 ;
        }
    }
}