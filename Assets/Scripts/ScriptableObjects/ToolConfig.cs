using System;
using Tables;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Tool", menuName = "ScriptableObjects/Tool", order = 3)]
    [Serializable]
    public class ToolConfig : ConfigWithCroponomPage {
        [Header("Tool")]
        public new string name;

        public ToolBuff buff;

        [Header("ToolShopProperties")]
        public int cost;

        public string explainText;
        public int buyAmount;

        [Header("HUDElements")]
        public ToolUIType toolUIType;

        public Sprite buffedIcon;

        [Header("FoodMarketProperties")]
        public bool isAlwaysAvailable;

        public Sprite FoodMarketSprite;
    }
}