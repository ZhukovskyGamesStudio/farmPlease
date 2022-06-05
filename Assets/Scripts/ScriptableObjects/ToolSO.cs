using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "ScriptableObjects/Tool", order = 3)]
[System.Serializable]
public class ToolSO : ScriptableObject
{
    public new string name;
    public ToolType type;

    [Header("ToolShopProperties")]
    public int cost;
    public string explainText;
    public int buyAmount;

    [Header("HUDElements")]
    public ToolUIType toolUIType;
    public Sprite buffedIcon;

    [Header("CroponomPage")]
    public string header;
    public string firstText;
    public Sprite firstSprite;
    public string secondText;
    public Sprite secondSprite;

    [Header("FoodMarketProperties")]
    public bool isAlwaysAvailable;
    public Sprite FoodMarketSprite;
}
