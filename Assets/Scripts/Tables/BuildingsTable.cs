using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingsTable : MonoBehaviour {
    public static BuildingsTable instance;
    public BuildingSO[] Buildings;

    public void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static BuildingSO BuildingByType(BuildingType type) {
        for (int i = 0; i < instance.Buildings.Length; i++)
            if (instance.Buildings[i].type == type)
                return instance.Buildings[i];
        Debug.Log("Нет класса Building под тип " + type);
        return null;
    }
}

[Serializable]
public enum BuildingType {
    None = -1,
    Biogen = 0,
    Freshener,
    Sprinkler,
    Sprinkler_target,
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