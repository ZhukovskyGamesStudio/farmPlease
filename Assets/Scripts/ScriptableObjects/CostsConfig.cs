using UnityEngine;

[CreateAssetMenu(fileName = "CostsConfig", menuName = "Scriptable Objects/CostsConfig")]
public class CostsConfig : ScriptableObject {
    public int[] LevelXpProgression;

    public int SeedsShopChangeCost = 5;
    public int ToolsShopChangeCost = 10;

    public int[] BuildingPriceProgression;
    public int CropPrice;
    public int ToolPrice;
}