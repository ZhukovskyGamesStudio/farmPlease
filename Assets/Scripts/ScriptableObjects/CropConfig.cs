using UnityEngine;

[CreateAssetMenu(fileName = "Crop", menuName = "ScriptableObjects/Crop", order = 2)]
public class CropConfig : ConfigWithCroponomPage {
    [Header("Crop")]
    public new string name;

    public Crop type;
    public Sprite VegSprite;

    [Header("SeedShopProperties")]
    public bool CanBeBought = true;

    public Sprite SeedSprite;
    public int cost;
    public string explainText;
    public int buyAmount;
    public int Rarity;
}