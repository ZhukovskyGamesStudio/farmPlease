using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/Building", order = 5)]
public class BuildingSO : ScriptableObject {
    public new string name;
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