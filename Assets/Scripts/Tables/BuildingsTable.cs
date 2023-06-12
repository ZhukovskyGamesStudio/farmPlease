using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tables
{
    public class BuildingsTable : MonoBehaviour {
        public static BuildingsTable Instance;
        public BuildingConfig[] Buildings;

        public void Awake() {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public static BuildingConfig BuildingByType(BuildingType type) {
            for (int i = 0; i < Instance.Buildings.Length; i++)
                if (Instance.Buildings[i].type == type)
                    return Instance.Buildings[i];
            UnityEngine.Debug.Log("Нет класса Building под тип " + type);
            return null;
        }
    }

    [Serializable]
    public enum BuildingType {
        None = -1,
        Biogen = 0,
        Freshener,
        Sprinkler,
        SprinklerTarget,
        SeedDoubler,
        Tractor
    }

    [Serializable]
    public class Building {
        public string name;
        public bool IsFakeBuilding;
        public BuildingType type;
        public Tile BuildingPanelTile;

        [Header("BuildingOffer")]
        public Sprite offerSprite;

        public string offerHeader;
        public string offerText;

        [Header("CroponomPage")]
        public string header;

        public string firstText;
        public Sprite firstSprite;
        public string secondText;
        public Sprite secondSprite;
    }
}