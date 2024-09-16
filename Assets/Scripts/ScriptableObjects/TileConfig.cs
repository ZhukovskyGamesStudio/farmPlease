using Tables;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Scriptable Objects/Tile", order = 1)]
    public class TileConfig : ScriptableObject {
        public TileType type;
        public TileBase TileBase;

        [Space(10)]
        public bool CanBeHoed;

        public bool CanBeSeeded;
        public bool CanBeWatered;
        public TileType WaterSwitch;
        public bool CanBeCollected;
        public Crop cropCollected;
        public int collectAmount;

        [HideInInspector]
        public Crop crop;

        [Space(10)]
        public bool CanBeNewDayed;

        public TileType NewDaySwitch;
        public bool CanBeErosioned;
        public bool CanBeClicked;

        [Space(10)]
        public int TIndex;

        [HideInInspector]
        public bool IsBuilding;

        [HideInInspector]
        public BuildingType BuildingType;
    }
}