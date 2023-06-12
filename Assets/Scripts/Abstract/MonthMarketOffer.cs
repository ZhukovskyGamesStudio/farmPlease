using System;
using UnityEngine;

[Serializable]
public class MonthMarketOffer {
    [Header("Monthmarket Offer")]
    public string Mname;

    public CropsType Ctype;
    public ToolBuff Ttype;
    public BuildingType Btype;
}