using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SmartTilemap : MonoBehaviour
{
	public static SmartTilemap instance = null;
	public Tilemap MainTilemap;
	public Tilemap BuildingTilemap;
	public ToolsAnimTilemap toolsAnimTilemap;

	[HideInInspector] public SeedShopScript seedShop;

	public TilesTable tilesTablePrefab;
	public Transform TilesHolder;
	public Vector3Int Playercoord;
	Dictionary<Vector3Int, SmartTile> tiles;

	public float animtime = 0.5f;
	Vector2Int fieldSizeJ = new(-13,13);
	Vector2Int fieldSizeI = new(-11,9);

	public int[] tileData;

	/**********/

	public void Awake()
	{
		if (instance == null)  		
			instance = this; 		
		else if (instance != this)
			Destroy(gameObject);
	}

    private void Start()
    {
		seedShop = UIScript.instance.ShopsPanel.seedShopScript;
	}

    public void Update()
	{
		Playercoord = MainTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}

   /* private void OnApplicationPause(bool pause)
    {
		if (pause)
			Time.timeScale = 100;
		else
			Time.timeScale = 1;
	}  
   */

    public void GenerateTiles()
    {
		tiles = new Dictionary<Vector3Int, SmartTile>();

		int circle = 0;
        int step = 0;
        int i = 0;
        Vector3Int curCoord = new Vector3Int(0, 0, 0);

        while (circle < 13)
        {

			GameObject tileObject = new GameObject();
			tileObject.transform.parent = TilesHolder;

			SmartTile smarttile = tileObject.AddComponent<SmartTile>();
			if (circle < 8)
			{
				smarttile.Init(this, TileType.Sand, curCoord);
				PlaceTile(curCoord, TileType.Sand);
			}
			else
			{
				smarttile.Init(this, TileType.Rocks, curCoord);
				PlaceTile(curCoord, TileType.Rocks);
			}

			tiles.Add(curCoord, smarttile);

			curCoord = Next(curCoord, circle, step);

			step++;
			if (step == circle * 6 || circle == 0)
			{
				circle++;
				step = 0;
			}

			if (i > 10000)
			{
				Debug.LogError("spent too much in while. Instant Break");
				break;
			}
		}
		/*
		for (int i = fieldSizeI.x; i < fieldSizeI.y; i++)
		{
			for (int j = fieldSizeJ.x; j < fieldSizeJ.y; j++)
			{

				Vector3Int position = new Vector3Int(j, i, 0);
				GameObject tileObject = new GameObject();
				tileObject.transform.parent = TilesHolder;
				SmartTile smarttile = tileObject.AddComponent<SmartTile>();
				if (i < -5 || i > 5 || j < -6 || j > 5)
				{
					smarttile.Init(this, TileType.Rocks, position);
					PlaceTile(position, TileType.Rocks);
				}
				else
				{
						smarttile.Init(this, TileType.Sand, position);
						PlaceTile(position, TileType.Sand);
				}

				tiles.Add(position, smarttile);
			}
		}  */
	}


	public void GenerateTilesWithData(string tilesString)
	{
		MainTilemap.ClearAllTiles();
		tiles =  new Dictionary<Vector3Int, SmartTile>();
		

		Vector2Int curCoord = new Vector2Int(0, 0);
  
		/***/

		int circle = 0;
		int step = 0;

		for (int i = 0; i < tilesString.Length; i += 2)
		{
			int typeVal = DBConverter.UTFToInt(tilesString[i] + "" + tilesString[i + 1]);

			Vector3Int position = new Vector3Int(curCoord.x, curCoord.y, 0);
			GameObject tileObject = new GameObject();
			tileObject.transform.parent = TilesHolder;
			SmartTile smarttile = tileObject.AddComponent<SmartTile>();

			smarttile.Init(this, (TileType)typeVal, position);
			PlaceTile(position, (TileType)typeVal);

			tiles.Add(position, smarttile);

			curCoord = DBConverter.Next(curCoord, circle, step);
			step++;
			if (step == circle * 6 || circle == 0)
			{
				circle++;
				step = 0;
			}
		}

		/*
		if (i < -5 || i > 5 || j < -6 || j > 5)
		{
			smarttile.Init(this, TileType.Rocks, position);
			PlaceTile(position, TileType.Rocks);
		} */
	}

	public void UpdateTilesWithData(int[] newTileData)
	{
		int xAmount = fieldSizeI.y - fieldSizeI.x;

		for (int i = 0; i < newTileData.Length; i++)
        {
			int x = i / xAmount + fieldSizeI.x;
			int y = i % xAmount + fieldSizeJ.x;

			Vector3Int position = new Vector3Int(y, x, 0);
			PlaceTile(position, (TileType)newTileData[i]);
		}
	}

	public Dictionary<Vector2Int, SmartTile> GetTiles()
    {
		Dictionary<Vector2Int, SmartTile> res = new Dictionary<Vector2Int, SmartTile>();

        foreach (var item in tiles)
        {
			res.Add(new Vector2Int(item.Key.x, item.Key.y), item.Value);

        }

		return res;
	}


	public string GetTilesData()
	{
		Dictionary<Vector2Int, SmartTile> tiles2 = new Dictionary<Vector2Int, SmartTile>();

        foreach (var item in tiles)
        {
			tiles2.Add(new Vector2Int(item.Key.x, item.Key.y), item.Value);
		}

		string str = DBConverter.FieldToString(tiles2);
		return str;
	}	  


	/**********/

	public IEnumerator NewDay()
	{
		SaveLoadManager.instance.Sequence(true);
		Dictionary<Vector3Int, SmartTile> tempTiles = new Dictionary<Vector3Int, SmartTile>(tiles);
		List<SmartTile> toNewDay = new List<SmartTile>();

		foreach (var smartTile in tempTiles)
		{
			if (smartTile.Value.CanbeNewDayed())
			{
				toNewDay.Add(smartTile.Value);	
			}
		}
		for (int i = 0; i < toNewDay.Count; i++)
		{
			yield return StartCoroutine(toNewDay[i].OnNeyDayed(animtime / 5));
		}
		SaveLoadManager.instance.Sequence(false);
	}

	public IEnumerator EndDayEvent(HappeningType happeningType)
	{
		SaveLoadManager.instance.Sequence(true);
		switch (happeningType)
		{
			case HappeningType.Erosion:
				yield return StartCoroutine(Erosion()); break;
			case HappeningType.Rain:
				yield return StartCoroutine(Rain()); break;
			case HappeningType.Wind:
				yield return StartCoroutine(InventoryManager.instance.WindyDay(this)); break;
			case HappeningType.Insects:
				yield return StartCoroutine(Insects()); break;
		}
		SaveLoadManager.instance.Sequence(false);
	}

	public void PlaceTile(Vector3Int coord, TileType type)
	{  	
		MainTilemap.SetTile(coord, TilesTable.TileByType(type).TileBase);
	}


    /**********/
    public bool BuildingCanBePlaced(BuildingType type, Vector3Int coord)
    {
        switch (type)
        {
            case BuildingType.Freshener:
            case BuildingType.Biogen:
            case BuildingType.Sprinkler:
            case BuildingType.SeedDoubler:
            case BuildingType.Tractor:
                SmartTile[] neighbors = GetHexNeighbors(coord);
                bool isBuilding = tiles[coord].IsBuilding() || neighbors[5].IsBuilding() || neighbors[0].IsBuilding() || neighbors[1].IsBuilding();	               
                bool isRocks = tiles[coord].type == TileType.Rocks || neighbors[5].type == TileType.Rocks || neighbors[0].type == TileType.Rocks || neighbors[1].type == TileType.Rocks;
                return !isBuilding && !isRocks;
            case BuildingType.Sprinkler_target:
                return !tiles[coord].IsBuilding() && !(tiles[coord].type == TileType.Rocks);
            default:
                Debug.Log("Wrong");
                return false;
        }
    }

    public void PlaceBuilding(BuildingType type, Vector3Int coord )
    {
		SmartTile[] neighbors = GetHexNeighbors(coord);
		switch (type)
        {
			case BuildingType.Biogen:
				tiles[coord].SwitchType(TileType.Biogen_Construction);					
				neighbors[5].SwitchType(TileType.Biogen_T1);
				neighbors[0].SwitchType(TileType.Biogen_T2);
				neighbors[1].SwitchType(TileType.Biogen_T3);  
				break;
			case BuildingType.Freshener:
				tiles[coord].SwitchType(TileType.Freshener_Construction); 				
				neighbors[5].SwitchType(TileType.Freshener_T1);
				neighbors[0].SwitchType(TileType.Freshener_T2);
				neighbors[1].SwitchType(TileType.Freshener_T3);
				break;
			case BuildingType.Sprinkler:
				tiles[coord].SwitchType(TileType.Sprinkler_Construction);
				neighbors[5].SwitchType(TileType.Sprinkler_T1);
				neighbors[0].SwitchType(TileType.Sprinkler_T2);
				neighbors[1].SwitchType(TileType.Sprinkler_T3);
				break;
			case BuildingType.Sprinkler_target:
				tiles[coord].SwitchType(TileType.Sprinkler_target);
				break;
			case BuildingType.SeedDoubler:
				tiles[coord].SwitchType(TileType.SeedDoubler_Construction);
				neighbors[5].SwitchType(TileType.SeedDoubler_T1);
				neighbors[0].SwitchType(TileType.SeedDoubler_T2);
				neighbors[1].SwitchType(TileType.SeedDoubler_T3);
				seedShop.SetAmbarCrop(CropsType.Weed);
				break;
			case BuildingType.Tractor:
				tiles[coord].SwitchType(TileType.Tractor_Construction);
				neighbors[5].SwitchType(TileType.Tractor_T1);
				neighbors[0].SwitchType(TileType.Tractor_T2);
				neighbors[1].SwitchType(TileType.Tractor_T3);
				break;
		}
    }

	public void DeactiveBuilding(BuildingType type, Vector3Int coord)
	{
		switch (type)
		{
			case BuildingType.Freshener:
			case BuildingType.Biogen:
			case BuildingType.Sprinkler:
			case BuildingType.SeedDoubler:
			case BuildingType.Tractor:
				tiles[coord].BecomeInactive();
				SmartTile[] neighbors = GetHexNeighbors(coord);
				neighbors[5].BecomeInactive();
				neighbors[0].BecomeInactive();
				neighbors[1].BecomeInactive();
				break;
			case BuildingType.Sprinkler_target:
				tiles[coord].BecomeInactive();
				break;
		}
	}

	public void ActiveBuilding(BuildingType type, Vector3Int coord)
	{
		switch (type)
		{
			case BuildingType.Freshener:
			case BuildingType.Biogen:
			case BuildingType.Sprinkler:
			case BuildingType.SeedDoubler:
			case BuildingType.Tractor:
				tiles[coord].BecomeActive();

				SmartTile[] neighbors = GetHexNeighbors(coord);
				neighbors[5].BecomeActive();
				neighbors[0].BecomeActive();
				neighbors[1].BecomeActive();
				break;
			case BuildingType.Sprinkler_target:
				tiles[coord].BecomeActive();
				break;
		}
	}

	public void RemoveBuilding(BuildingType type, Vector3Int coord)
	{
		switch (type)
		{
			case BuildingType.Freshener:
			case BuildingType.Biogen:
			case BuildingType.Sprinkler:
			case BuildingType.SeedDoubler:
			case BuildingType.Tractor:
				tiles[coord].SwitchType(TileType.Sand);
				SmartTile[] neighbors = GetHexNeighbors(coord);
				neighbors[5].SwitchType(TileType.Sand);
				neighbors[0].SwitchType(TileType.Sand);
				neighbors[1].SwitchType(TileType.Sand);
				break;
			case BuildingType.Sprinkler_target:
				tiles[coord].SwitchType(TileType.Sand);
				break;
		}
	}


	/**********/

	// 0 - hoe; 1 - seed; 2 - water; 3 - collect
	public bool AvailabilityCheck(string actionName) {

        switch (actionName)
        {
			case "building":
				return tiles[Playercoord].IsBuilding();
			case "click":
				return tiles[Playercoord].CanBeClicked();
			case "hoe":
				return tiles[Playercoord].CanBeHoed();
			case "seed":
				return tiles[Playercoord].CanBeSeeded();
			case "water":
				return tiles[Playercoord].CanBeWatered();
			case "collect":
				return tiles[Playercoord].CanBeCollected();
		}
		Debug.Log("Error here " + actionName);
		return false;
	}

	/**********/

	public IEnumerator NewDayTile()
	{
		yield return StartCoroutine(tiles[Playercoord].OnClicked(animtime));
	}

	public IEnumerator ClickTile()
	{
		yield return StartCoroutine(tiles[Playercoord].OnClicked(animtime));
	}


	public IEnumerator SeedTile( CropsType crop)
	{
		yield return StartCoroutine(tiles[Playercoord].OnSeeded(crop, animtime));
	}

	public IEnumerator CollectTile()
	{
		yield return StartCoroutine(tiles[Playercoord].OnCollected(InventoryManager.instance.IsToolWorking(ToolType.Greenscythe),animtime/3));
	}

	public IEnumerator HoeTile()
	{
		yield return StartCoroutine(tiles[Playercoord].OnHoed(animtime));
	}

	public IEnumerator WaterTile()
	{
		yield return StartCoroutine(tiles[Playercoord].OnWatered(animtime));
	}

	public IEnumerator HoeRandomNeighbor(Vector3Int center)
	{
		SmartTile[] neighbors = GetHexNeighbors(center);
		List<SmartTile> neighborsList = new List<SmartTile>();

		for (int i = 0; i < neighbors.Length; i++)
		{
			if (neighbors[i].CanBeHoed())
				neighborsList.Add(neighbors[i]);
		}

		if (neighborsList.Count > 0)
			yield return StartCoroutine(neighborsList[UnityEngine.Random.Range(0, neighborsList.Count)].OnHoed(animtime));
	}

	public IEnumerator Erosion()
	{
		Dictionary<Vector3Int, SmartTile> tempTiles = new Dictionary<Vector3Int, SmartTile>(tiles);
		List< SmartTile> toErosion = new List<SmartTile>();
		foreach (var smartTile in tempTiles)
		{
			if (smartTile.Value.CanBeErosioned())
				if(smartTile.Value.type != TileType.Freshener_full)
					toErosion.Add(smartTile.Value);
				else
                {
					yield return StartCoroutine(smartTile.Value.OnErosioned(animtime / 5));
					yield break;
				}
				
		}

		for (int i = 0; i < toErosion.Count; i++)
		{
			yield return StartCoroutine(toErosion[i].OnErosioned(animtime / 5));
		}
	}

	public IEnumerator Rain()
	{
		Dictionary<Vector3Int, SmartTile> tempTiles = new Dictionary<Vector3Int, SmartTile>(tiles);
		foreach (var smartTile in tempTiles)
		{
			if (smartTile.Value.CanBeWatered())
				yield return StartCoroutine(smartTile.Value.OnWatered(animtime / 5));
		}
	}

	public IEnumerator Insects()
	{
		
		List<Vector3Int> FlycatcherPosition = new List<Vector3Int>();
		foreach (var smartTile in tiles)
		{
			if (smartTile.Value.type == TileType.FlycatherSeed_3)
			{
				FlycatcherPosition.Add(smartTile.Key);
			}
		}

		if(FlycatcherPosition.Count > 0)
		{
			for (int i = 0; i < FlycatcherPosition.Count; i++)
			{
				yield return StartCoroutine(tiles[FlycatcherPosition[i]].OnInsected(animtime));
			}

			SmartTile[] alltiles = GetAllTiles();
			List<SmartTile> toSeedList = new List<SmartTile>();
			for (int i = 0; i < alltiles.Length; i++)
			{
				if (alltiles[i].CanBeCollected())
                {
					alltiles[i].BecomeInactive();
					toSeedList.Add(alltiles[i]);
				}	  					
			}

			for (int i = 0; i < toSeedList.Count; i++)
			{
				yield return new WaitForSeconds(animtime / 5);
				InventoryManager.instance.CollectCrop(CropsType.Flycatcher, 1);
				toSeedList[i].BecomeActive();
			}

			for (int i = 0; i < FlycatcherPosition.Count; i++)
			{
				tiles[FlycatcherPosition[i]].BecomeActive();
			}


		}
		else
		{
			Dictionary<Vector3Int, SmartTile> tempTiles = new Dictionary<Vector3Int, SmartTile>(tiles);
			foreach (var smartTile in tempTiles)
			{
				if (smartTile.Value.CanBeCollected())
					yield return StartCoroutine(smartTile.Value.OnInsected(animtime / 5));
			}
		}		
	}

	public SmartTile[] GetAllTiles()
	{
		SmartTile[] resTiles = new SmartTile[tiles.Count];

		int i = 0;
		foreach (var tile in tiles)
		{
			resTiles[i] = tile.Value;
			i++;
		}

		return resTiles;
	}

    public SmartTile[] GetAllTiles(TileType type)
    {
        List<SmartTile> tilesL = new List<SmartTile>();

        foreach (var tile in tiles)
        {
            if (tile.Value.type == type)
                tilesL.Add(tile.Value);
        }

        return tilesL.ToArray();
    }


    public bool HasTile(TileType[] type)
	{

		SmartTile[] resTiles = GetAllTiles();

		foreach (var tile in resTiles)
		{
            for (int i = 0; i < type.Length; i++)             
				if (tile.type == type[i])
					return true;
		}
		return false;
	}

	public SmartTile GetTileFromAll(TileType type)
	{
		SmartTile[] resTiles = GetAllTiles();

        foreach (var tile in resTiles)
        {
            if (tile.type == type)
                return tile;
        }
        Debug.Log("trouble");
		return null;
	}

	public SmartTile[] GetHexNeighbors(Vector3Int center)
	{
		SmartTile[] neighbors = new SmartTile[6];

		if (center.y % 2 == 0)
		{
			neighbors[0] = tiles[center + new Vector3Int(0, 1, 0)];
			neighbors[1] = tiles[center + new Vector3Int(1, 0, 0)];
			neighbors[2] = tiles[center + new Vector3Int(0, -1, 0)];
			neighbors[3] = tiles[center + new Vector3Int(-1, -1, 0)];
			neighbors[4] = tiles[center + new Vector3Int(-1, 0, 0)];
			neighbors[5] = tiles[center + new Vector3Int(-1, 1, 0)];
		}
		else
		{			 			
			neighbors[0] = tiles[center + new Vector3Int(1, 1, 0)];
			neighbors[1] = tiles[center + new Vector3Int(1, 0, 0)];
			neighbors[2] = tiles[center + new Vector3Int(1, -1, 0)];
			neighbors[3] = tiles[center + new Vector3Int(0, -1, 0)];
			neighbors[4] = tiles[center + new Vector3Int(-1, 0, 0)];
			neighbors[5] = tiles[center + new Vector3Int(0, 1, 0)];
		}

		return neighbors;
	}

	public Vector3Int[] GetHexCoordinates(Vector3Int center)
	{
		Vector3Int[] neighbors = new Vector3Int[6];

		if (center.y % 2 == 0)
		{
			neighbors[0] = center + new Vector3Int(0, 1, 0);
			neighbors[1] = center + new Vector3Int(1, 0, 0);
			neighbors[2] = center + new Vector3Int(0, -1, 0);
			neighbors[3] = center + new Vector3Int(-1, -1, 0);
			neighbors[4] = center + new Vector3Int(-1, 0, 0);
			neighbors[5] = center + new Vector3Int(-1, 1, 0);
		}
		else
		{
			neighbors[0] = center + new Vector3Int(1, 1, 0);
			neighbors[1] = center + new Vector3Int(1, 0, 0);
			neighbors[2] = center + new Vector3Int(1, -1, 0);
			neighbors[3] = center + new Vector3Int(0, -1, 0);
			neighbors[4] = center + new Vector3Int(-1, 0, 0);
			neighbors[5] = center + new Vector3Int(0, 1, 0);
		}
		return neighbors;
	}

	Vector3Int Next(Vector3Int now, int circle, int step)
	{
		if (circle == 0)
			return GetHexNeighbor(now, 0);

		if (step == circle * 6 - 1)
			return GetHexNeighbor(GetHexNeighbor(now, 5), 0);

		step /= circle;

		switch (step)
		{
			case 0: return GetHexNeighbor(now, 4);
			case 1: return GetHexNeighbor(now, 3);
			case 2: return GetHexNeighbor(now, 2);
			case 3: return GetHexNeighbor(now, 1);
			case 4: return GetHexNeighbor(now, 0);
			case 5: return GetHexNeighbor(now, 5);
		}

		Debug.LogError("Not meant to be here");

		return now + new Vector3Int(0, 0,1);
	}

	Vector3Int GetHexNeighbor(Vector3Int center, int index)
	{
		Vector3Int[] neighbors = new Vector3Int[6];

		if (center.y % 2 == 0)
		{
			neighbors[0] = center + new Vector3Int(0, 1,0);
			neighbors[1] = center + new Vector3Int(1, 0,0);
			neighbors[2] = center + new Vector3Int(0, -1,0);
			neighbors[3] = center + new Vector3Int(-1, -1,0);
			neighbors[4] = center + new Vector3Int(-1, 0,0);
			neighbors[5] = center + new Vector3Int(-1, 1,0);
		}
		else
		{
			neighbors[0] = center + new Vector3Int(1, 1,0);
			neighbors[1] = center + new Vector3Int(1, 0,0);
			neighbors[2] = center + new Vector3Int(1, -1,0);
			neighbors[3] = center + new Vector3Int(0, -1,0);
			neighbors[4] = center + new Vector3Int(-1, 0,0);
			neighbors[5] = center + new Vector3Int(0, 1,0);
		}

		return neighbors[index];
	}


	public SmartTile GetRandomNeighbor(Vector3Int center)
	{
		int i = UnityEngine.Random.Range(0, 6);
		return GetHexNeighbors(center)[i];
	}

	public List<SmartTile> GetNeighborsWithType(Vector3Int center, TileType type)
	{
		SmartTile[] neighbors = GetHexNeighbors(center);
		List<SmartTile> neighborsList = new List<SmartTile>();
        for (int i = 0; i < neighbors.Length; i++)
        {
			if (neighbors[i].type == type)
				neighborsList.Add(neighbors[i]);
        }

		return neighborsList;
	}

	public List<SmartTile> GetNeighborsWithType(Vector3Int center, TileType type1, TileType type2)
	{
		SmartTile[] neighbors = GetHexNeighbors(center);
		List<SmartTile> neighborsList = new List<SmartTile>();
		for (int i = 0; i < neighbors.Length; i++)
		{
			if ((neighbors[i].type == type1 || neighbors[i].type == type2))
				neighborsList.Add(neighbors[i]);
		}

		return neighborsList;
	}

	public SmartTile GetTile(Vector3Int center)
	{
		return tiles[center];
	}

	public SmartTile GetPlayerTile()
	{
		return tiles[Playercoord];
	}

	public Vector3Int GetBuildingCoordinatesByPart( Vector3Int coord)
	{
		
		Vector3Int[] neighbors = GetHexCoordinates(coord);
		
		TileData data = TilesTable.TileByType(tiles[coord].type);
		if (data.IsBuilding)
			switch (data.TIndex)
			{
				case 0:
					return coord;
				case 1:
					return neighbors[2];
				case 2:
					return neighbors[3];
				case 3:
					return neighbors[4];
			}
		
		Debug.Log("Trouble");
		return Vector3Int.zero;

	}

	public SmartTile GetBuildingByPart(Vector3Int coord)
	{
		return tiles[GetBuildingCoordinatesByPart(coord)];
	}


}