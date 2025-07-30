using Tables;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Crop", menuName = "Scriptable Objects/Crop", order = 2)]
    public class CropConfig : ConfigWithCroponomPage {
        [Header("Crop")]
        public new string name;

        public Crop type;
        public Sprite VegSprite;

        [Header("SeedShopProperties")]
        public bool CanBeBought = true;

        public Sprite SeedSprite;
        public int cost;
        [LocalizationKey("Croponom")]
        public string explainTextLoc;
        public string explainText;
        public int buyAmount;
        public int Rarity;
        public override string GetUnlockable() => type.ToString();
    }
}