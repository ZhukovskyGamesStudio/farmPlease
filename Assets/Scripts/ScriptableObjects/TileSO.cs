using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class TileSO : ScriptableObject
{
	public TileType type;
	public TileBase TileBase;
	[Space(10)]
	public bool CanBeHoed;
	public bool CanBeSeeded;
	public bool CanBeWatered;
	public TileType WaterSwitch;
	public bool CanBeCollected;
	public CropsType CropCollected;
	public int collectAmount;
	[HideInInspector]
	public CropsType CropType;
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
