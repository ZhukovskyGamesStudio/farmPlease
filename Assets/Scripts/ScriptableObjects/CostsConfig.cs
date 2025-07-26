using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "CostsConfig", menuName = "Scriptable Objects/CostsConfig")]
public class CostsConfig : ScriptableObject {
    public int[] LevelXpProgression;

    public int SeedsShopChangeCost = 5;
    public int ToolsShopChangeCost = 10;

    public int[] BuildingPriceProgression;
    public int CropPrice;
    public int ToolPrice;
    public float MunitesForOneChargeRefill;

    public float MinutesGoldenClockWorks = 30f;
    public float MinutesGoldenScytheWorks = 60f;

    public float HoursQuestsChange = 24;
    public int LevelToUnlockDaily = 3;

    public int XpForBuySeed = 1;
    public int XpForBuyTool = 3;
    public int XpForBaseAction = 1;


    public int LevelToRateUs = 3;
    public int LevelToStartInters = 3;
}