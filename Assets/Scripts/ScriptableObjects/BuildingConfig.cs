using Tables;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building", order = 5)]
    public class BuildingConfig : ConfigWithCroponomPage {
        public new string name;
        public bool IsFakeBuilding;
        public BuildingType type;
        public Tile BuildingPanelTile;

        [Header("BuildingOffer")]
        public Sprite offerSprite;

        public string offerHeader;
        public string offerText;

        public override string GetUnlockable() {
            throw new System.NotImplementedException();
        }
    }
}