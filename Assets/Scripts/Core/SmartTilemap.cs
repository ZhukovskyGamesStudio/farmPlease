using System.Collections;
using System.Collections.Generic;
using Database;
using Managers;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = Managers.Debug;
using TileData = Tables.TileData;

public class SmartTilemap : MonoBehaviour {
    public static SmartTilemap Instance;
    public Tilemap MainTilemap;
    public Tilemap BuildingTilemap;
    public ToolsAnimTilemap toolsAnimTilemap;

    [HideInInspector]
    public SeedShopView seedShop;

    public TilesTable tilesTablePrefab;
    public Transform TilesHolder;
    public Vector3Int Playercoord;

    public float animtime = 0.5f;
    private const int TILES_RADIUS = 11;
    private const int STARTING_CIRCLE_RADIUS = 6;
    public int[] tileData;
    private Vector2Int _fieldSizeI = new(-11, 9);
    private Vector2Int _fieldSizeJ = new(-13, 13);
    private Dictionary<Vector3Int, SmartTile> _tiles;
    private Camera _mainCamera;
    /**********/

    public void Awake() {
        if (Instance == null) {
            Instance = this;
            _mainCamera = Camera.main;
        }
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start() {
        seedShop = UIHud.Instance.ShopsPanel.seedShopView;
    }

    public void Update() {
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x > 1000000) {
            //wraps unimportant editor bug Screen position out of view frustum (screen pos inf, -inf, 0.000000) 
            mousePos = Vector3.zero;
        }
        Playercoord = MainTilemap.WorldToCell(_mainCamera.ScreenToWorldPoint(mousePos));
    }

    public void GenerateTiles() {
        _tiles = new Dictionary<Vector3Int, SmartTile>();

        int circle = 0;
        int step = 0;
        int i = 0;
        Vector3Int curCoord = new(0, 0, 0);

        while (circle < TILES_RADIUS) {
            GameObject tileObject = new();
            tileObject.transform.parent = TilesHolder;

            SmartTile smarttile = tileObject.AddComponent<SmartTile>();
            if (circle < STARTING_CIRCLE_RADIUS) {
                smarttile.Init(this, TileType.Sand, curCoord);
                PlaceTile(curCoord, TileType.Sand);
            } else {
                smarttile.Init(this, TileType.Rocks, curCoord);
                PlaceTile(curCoord, TileType.Rocks);
            }

            _tiles.Add(curCoord, smarttile);

            curCoord = Next(curCoord, circle, step);

            step++;
            if (step == circle * 6 || circle == 0) {
                circle++;
                step = 0;
            }

            if (i > 10000) {
                UnityEngine.Debug.LogError("spent too much in while. Instant Break");
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

    public void GenerateTilesWithData(string tilesString) {
        MainTilemap.ClearAllTiles();
        _tiles = new Dictionary<Vector3Int, SmartTile>();

        Vector2Int curCoord = new(0, 0);

        /***/

        int circle = 0;
        int step = 0;

        for (int i = 0; i < tilesString.Length; i += 2) {
            int typeVal = DBConverter.UTFToInt(tilesString[i] + "" + tilesString[i + 1]);

            Vector3Int position = new(curCoord.x, curCoord.y, 0);
            GameObject tileObject = new();
            tileObject.transform.parent = TilesHolder;
            SmartTile smarttile = tileObject.AddComponent<SmartTile>();

            smarttile.Init(this, (TileType) typeVal, position);
            PlaceTile(position, (TileType) typeVal);

            _tiles.Add(position, smarttile);

            curCoord = DBConverter.Next(curCoord, circle, step);
            step++;
            if (step == circle * 6 || circle == 0) {
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

    public void UpdateTilesWithData(int[] newTileData) {
        int xAmount = _fieldSizeI.y - _fieldSizeI.x;

        for (int i = 0; i < newTileData.Length; i++) {
            int x = i / xAmount + _fieldSizeI.x;
            int y = i % xAmount + _fieldSizeJ.x;

            Vector3Int position = new(y, x, 0);
            PlaceTile(position, (TileType) newTileData[i]);
        }
    }

    public Dictionary<Vector2Int, SmartTile> GetTiles() {
        Dictionary<Vector2Int, SmartTile> res = new();

        foreach (KeyValuePair<Vector3Int, SmartTile> item in _tiles)
            res.Add(new Vector2Int(item.Key.x, item.Key.y), item.Value);

        return res;
    }

    public string GetTilesData() {
        Dictionary<Vector2Int, SmartTile> tiles2 = new();

        foreach (KeyValuePair<Vector3Int, SmartTile> item in _tiles)
            tiles2.Add(new Vector2Int(item.Key.x, item.Key.y), item.Value);

        string str = DBConverter.FieldToString(tiles2);
        return str;
    }

    /**********/

    public IEnumerator NewDay(HappeningType type) {
        SetHappeningType(type);
        SaveLoadManager.Instance.Sequence(true);
        Dictionary<Vector3Int, SmartTile> tempTiles = new(_tiles);
        List<SmartTile> toNewDay = new();

        foreach (KeyValuePair<Vector3Int, SmartTile> smartTile in tempTiles)
            if (smartTile.Value.CanbeNewDayed())
                toNewDay.Add(smartTile.Value);
        for (int i = 0; i < toNewDay.Count; i++) yield return StartCoroutine(toNewDay[i].OnNeyDayed(animtime / 5));
        yield return StartCoroutine(HappeningSequence());
        SaveLoadManager.Instance.Sequence(false);
    }

    private HappeningType _happeningType;
    public void SetHappeningType(HappeningType happeningType) {
        _happeningType = happeningType;
    }
    
    public IEnumerator HappeningSequence() {
        SaveLoadManager.Instance.Sequence(true);
        switch (_happeningType) {
            case HappeningType.Erosion:
                yield return StartCoroutine(Erosion());
                break;

            case HappeningType.Rain:
                yield return StartCoroutine(Rain());
                break;

            case HappeningType.Wind:
                yield return StartCoroutine(InventoryManager.Instance.WindyDay(this));
                break;

            case HappeningType.Insects:
                yield return StartCoroutine(Insects());
                break;
        }

        SaveLoadManager.Instance.Sequence(false);
    }

    public void PlaceTile(Vector3Int coord, TileType type) {
        MainTilemap.SetTile(coord, TilesTable.TileByType(type).TileBase);
    }

    /**********/
    public bool BuildingCanBePlaced(BuildingType type, Vector3Int coord) {
        switch (type) {
            case BuildingType.Freshener:
            case BuildingType.Biogen:
            case BuildingType.Sprinkler:
            case BuildingType.SeedDoubler:
            case BuildingType.Tractor:
                SmartTile[] neighbors = GetHexNeighbors(coord);
                bool isBuilding = _tiles[coord].IsBuilding() || neighbors[5].IsBuilding() || neighbors[0].IsBuilding() ||
                                  neighbors[1].IsBuilding();
                bool isRocks = _tiles[coord].type == TileType.Rocks || neighbors[5].type == TileType.Rocks ||
                               neighbors[0].type == TileType.Rocks || neighbors[1].type == TileType.Rocks;
                return !isBuilding && !isRocks;

            case BuildingType.SprinklerTarget:
                return !_tiles[coord].IsBuilding() && !(_tiles[coord].type == TileType.Rocks);

            default:
                UnityEngine.Debug.Log("Wrong");
                return false;
        }
    }

    public void PlaceBuilding(BuildingType type, Vector3Int coord) {
        SmartTile[] neighbors = GetHexNeighbors(coord);
        switch (type) {
            case BuildingType.Biogen:
                _tiles[coord].SwitchType(TileType.BiogenConstruction);
                neighbors[5].SwitchType(TileType.BiogenT1);
                neighbors[0].SwitchType(TileType.BiogenT2);
                neighbors[1].SwitchType(TileType.BiogenT3);
                break;

            case BuildingType.Freshener:
                _tiles[coord].SwitchType(TileType.FreshenerConstruction);
                neighbors[5].SwitchType(TileType.FreshenerT1);
                neighbors[0].SwitchType(TileType.FreshenerT2);
                neighbors[1].SwitchType(TileType.FreshenerT3);
                break;

            case BuildingType.Sprinkler:
                _tiles[coord].SwitchType(TileType.SprinklerConstruction);
                neighbors[5].SwitchType(TileType.SprinklerT1);
                neighbors[0].SwitchType(TileType.SprinklerT2);
                neighbors[1].SwitchType(TileType.SprinklerT3);
                break;

            case BuildingType.SprinklerTarget:
                _tiles[coord].SwitchType(TileType.SprinklerTarget);
                break;

            case BuildingType.SeedDoubler:
                _tiles[coord].SwitchType(TileType.SeedDoublerConstruction);
                neighbors[5].SwitchType(TileType.SeedDoublerT1);
                neighbors[0].SwitchType(TileType.SeedDoublerT2);
                neighbors[1].SwitchType(TileType.SeedDoublerT3);
                seedShop.SetAmbarCrop(Crop.Weed);
                break;

            case BuildingType.Tractor:
                _tiles[coord].SwitchType(TileType.TractorConstruction);
                neighbors[5].SwitchType(TileType.TractorT1);
                neighbors[0].SwitchType(TileType.TractorT2);
                neighbors[1].SwitchType(TileType.TractorT3);
                break;
        }
    }

    public void DeactiveBuilding(BuildingType type, Vector3Int coord) {
        switch (type) {
            case BuildingType.Freshener:
            case BuildingType.Biogen:
            case BuildingType.Sprinkler:
            case BuildingType.SeedDoubler:
            case BuildingType.Tractor:
                _tiles[coord].BecomeInactive();
                SmartTile[] neighbors = GetHexNeighbors(coord);
                neighbors[5].BecomeInactive();
                neighbors[0].BecomeInactive();
                neighbors[1].BecomeInactive();
                break;

            case BuildingType.SprinklerTarget:
                _tiles[coord].BecomeInactive();
                break;
        }
    }

    public void ActiveBuilding(BuildingType type, Vector3Int coord) {
        switch (type) {
            case BuildingType.Freshener:
            case BuildingType.Biogen:
            case BuildingType.Sprinkler:
            case BuildingType.SeedDoubler:
            case BuildingType.Tractor:
                _tiles[coord].BecomeActive();

                SmartTile[] neighbors = GetHexNeighbors(coord);
                neighbors[5].BecomeActive();
                neighbors[0].BecomeActive();
                neighbors[1].BecomeActive();
                break;

            case BuildingType.SprinklerTarget:
                _tiles[coord].BecomeActive();
                break;
        }
    }

    public void RemoveBuilding(BuildingType type, Vector3Int coord) {
        switch (type) {
            case BuildingType.Freshener:
            case BuildingType.Biogen:
            case BuildingType.Sprinkler:
            case BuildingType.SeedDoubler:
            case BuildingType.Tractor:
                _tiles[coord].SwitchType(TileType.Sand);
                SmartTile[] neighbors = GetHexNeighbors(coord);
                neighbors[5].SwitchType(TileType.Sand);
                neighbors[0].SwitchType(TileType.Sand);
                neighbors[1].SwitchType(TileType.Sand);
                break;

            case BuildingType.SprinklerTarget:
                _tiles[coord].SwitchType(TileType.Sand);
                break;
        }
    }

    /**********/

    // 0 - hoe; 1 - seed; 2 - water; 3 - collect
    public bool AvailabilityCheck(string actionName) {
        if (!_tiles.ContainsKey(Playercoord)) {
            Debug.Instance.Log("No tile with this coordinates " + Playercoord);
            return false;
        }
        switch (actionName) {
            case "building":
                return _tiles[Playercoord].IsBuilding();

            case "click":
                return _tiles[Playercoord].CanBeClicked();

            case "hoe":
                return _tiles[Playercoord].CanBeHoed();

            case "seed":
                return _tiles[Playercoord].CanBeSeeded();

            case "water":
                return _tiles[Playercoord].CanBeWatered();

            case "collect":
                return _tiles[Playercoord].CanBeCollected();
        }

        UnityEngine.Debug.Log("Error here " + actionName);
        return false;
    }

    /**********/

    public IEnumerator NewDayTile() {
        yield return StartCoroutine(_tiles[Playercoord].OnClicked(animtime));
    }

    public IEnumerator ClickTile() {
        yield return StartCoroutine(_tiles[Playercoord].OnClicked(animtime));
        yield return StartCoroutine(HappeningSequence());
    }

    public IEnumerator SeedTile(Crop crop) {
        yield return StartCoroutine(_tiles[Playercoord].OnSeeded(crop, animtime));
        yield return StartCoroutine(HappeningSequence());
    }

    public IEnumerator CollectTile() {
        yield return StartCoroutine(_tiles[Playercoord]
            .OnCollected(InventoryManager.Instance.IsToolWorking(ToolBuff.Greenscythe), animtime / 3));
        yield return StartCoroutine(HappeningSequence());
    }

    public IEnumerator HoeTile() {
        yield return StartCoroutine(_tiles[Playercoord].OnHoed(animtime));
        yield return StartCoroutine(HappeningSequence());
    }

    public IEnumerator WaterTile() {
        yield return StartCoroutine(_tiles[Playercoord].OnWatered(animtime));
        yield return StartCoroutine(HappeningSequence());
    }

    public IEnumerator HoeRandomNeighbor(Vector3Int center) {
        SmartTile[] neighbors = GetHexNeighbors(center);
        List<SmartTile> neighborsList = new();

        for (int i = 0; i < neighbors.Length; i++)
            if (neighbors[i].CanBeHoed())
                neighborsList.Add(neighbors[i]);

        if (neighborsList.Count > 0)
            yield return StartCoroutine(neighborsList[Random.Range(0, neighborsList.Count)].OnHoed(animtime));
        yield return StartCoroutine(HappeningSequence());
    }

    public IEnumerator Erosion() {
        Dictionary<Vector3Int, SmartTile> tempTiles = new(_tiles);
        List<SmartTile> toErosion = new();
        foreach (KeyValuePair<Vector3Int, SmartTile> smartTile in tempTiles)
            if (smartTile.Value.CanBeErosioned())
                if (smartTile.Value.type != TileType.FreshenerFull) {
                    toErosion.Add(smartTile.Value);
                } else {
                    yield return StartCoroutine(smartTile.Value.OnErosioned(animtime / 5));
                    yield break;
                }

        for (int i = 0; i < toErosion.Count; i++) yield return StartCoroutine(toErosion[i].OnErosioned(animtime / 5));
    }

    public IEnumerator Rain() {
        Dictionary<Vector3Int, SmartTile> tempTiles = new(_tiles);
        foreach (KeyValuePair<Vector3Int, SmartTile> smartTile in tempTiles)
            if (smartTile.Value.CanBeWatered())
                yield return StartCoroutine(smartTile.Value.OnWatered(animtime / 5));
    }

    public IEnumerator Insects() {
        List<Vector3Int> flycatcherPosition = new();
        foreach (KeyValuePair<Vector3Int, SmartTile> smartTile in _tiles)
            if (smartTile.Value.type == TileType.FlycatherSeed3)
                flycatcherPosition.Add(smartTile.Key);

        if (flycatcherPosition.Count > 0) {
            for (int i = 0; i < flycatcherPosition.Count; i++)
                yield return StartCoroutine(_tiles[flycatcherPosition[i]].OnInsected(animtime));

            SmartTile[] alltiles = GetAllTiles();
            List<SmartTile> toSeedList = new();
            for (int i = 0; i < alltiles.Length; i++)
                if (alltiles[i].CanBeCollected()) {
                    alltiles[i].BecomeInactive();
                    toSeedList.Add(alltiles[i]);
                }

            for (int i = 0; i < toSeedList.Count; i++) {
                yield return new WaitForSeconds(animtime / 5);
                toSeedList[i].HarvestCrop(Crop.Flycatcher, 1);
                toSeedList[i].BecomeActive();
            }

            for (int i = 0; i < flycatcherPosition.Count; i++) _tiles[flycatcherPosition[i]].BecomeActive();
        } else {
            Dictionary<Vector3Int, SmartTile> tempTiles = new(_tiles);
            foreach (KeyValuePair<Vector3Int, SmartTile> smartTile in tempTiles)
                if (smartTile.Value.CanBeCollected())
                    yield return StartCoroutine(smartTile.Value.OnInsected(animtime / 5));
        }
    }

    public SmartTile[] GetAllTiles() {
        SmartTile[] resTiles = new SmartTile[_tiles.Count];

        int i = 0;
        foreach (KeyValuePair<Vector3Int, SmartTile> tile in _tiles) {
            resTiles[i] = tile.Value;
            i++;
        }

        return resTiles;
    }

    public SmartTile[] GetAllTiles(TileType type) {
        List<SmartTile> tilesL = new();

        foreach (KeyValuePair<Vector3Int, SmartTile> tile in _tiles)
            if (tile.Value.type == type)
                tilesL.Add(tile.Value);

        return tilesL.ToArray();
    }

    public bool HasTile(TileType[] type) {
        SmartTile[] resTiles = GetAllTiles();

        foreach (SmartTile tile in resTiles)
            for (int i = 0; i < type.Length; i++)
                if (tile.type == type[i])
                    return true;
        return false;
    }

    public SmartTile GetTileFromAll(TileType type) {
        SmartTile[] resTiles = GetAllTiles();

        foreach (SmartTile tile in resTiles)
            if (tile.type == type)
                return tile;
        UnityEngine.Debug.Log("trouble");
        return null;
    }

    public SmartTile[] GetHexNeighbors(Vector3Int center) {
        SmartTile[] neighbors = new SmartTile[6];

        if (center.y % 2 == 0) {
            neighbors[0] = _tiles[center + new Vector3Int(0, 1, 0)];
            neighbors[1] = _tiles[center + new Vector3Int(1, 0, 0)];
            neighbors[2] = _tiles[center + new Vector3Int(0, -1, 0)];
            neighbors[3] = _tiles[center + new Vector3Int(-1, -1, 0)];
            neighbors[4] = _tiles[center + new Vector3Int(-1, 0, 0)];
            neighbors[5] = _tiles[center + new Vector3Int(-1, 1, 0)];
        } else {
            neighbors[0] = _tiles[center + new Vector3Int(1, 1, 0)];
            neighbors[1] = _tiles[center + new Vector3Int(1, 0, 0)];
            neighbors[2] = _tiles[center + new Vector3Int(1, -1, 0)];
            neighbors[3] = _tiles[center + new Vector3Int(0, -1, 0)];
            neighbors[4] = _tiles[center + new Vector3Int(-1, 0, 0)];
            neighbors[5] = _tiles[center + new Vector3Int(0, 1, 0)];
        }

        return neighbors;
    }

    public Vector3Int[] GetHexCoordinates(Vector3Int center) {
        Vector3Int[] neighbors = new Vector3Int[6];

        if (center.y % 2 == 0) {
            neighbors[0] = center + new Vector3Int(0, 1, 0);
            neighbors[1] = center + new Vector3Int(1, 0, 0);
            neighbors[2] = center + new Vector3Int(0, -1, 0);
            neighbors[3] = center + new Vector3Int(-1, -1, 0);
            neighbors[4] = center + new Vector3Int(-1, 0, 0);
            neighbors[5] = center + new Vector3Int(-1, 1, 0);
        } else {
            neighbors[0] = center + new Vector3Int(1, 1, 0);
            neighbors[1] = center + new Vector3Int(1, 0, 0);
            neighbors[2] = center + new Vector3Int(1, -1, 0);
            neighbors[3] = center + new Vector3Int(0, -1, 0);
            neighbors[4] = center + new Vector3Int(-1, 0, 0);
            neighbors[5] = center + new Vector3Int(0, 1, 0);
        }

        return neighbors;
    }

    private Vector3Int Next(Vector3Int now, int circle, int step) {
        if (circle == 0)
            return GetHexNeighbor(now, 0);

        if (step == circle * 6 - 1)
            return GetHexNeighbor(GetHexNeighbor(now, 5), 0);

        step /= circle;

        switch (step) {
            case 0: return GetHexNeighbor(now, 4);

            case 1: return GetHexNeighbor(now, 3);

            case 2: return GetHexNeighbor(now, 2);

            case 3: return GetHexNeighbor(now, 1);

            case 4: return GetHexNeighbor(now, 0);

            case 5: return GetHexNeighbor(now, 5);
        }

        UnityEngine.Debug.LogError("Not meant to be here");

        return now + new Vector3Int(0, 0, 1);
    }

    private Vector3Int GetHexNeighbor(Vector3Int center, int index) {
        Vector3Int[] neighbors = new Vector3Int[6];

        if (center.y % 2 == 0) {
            neighbors[0] = center + new Vector3Int(0, 1, 0);
            neighbors[1] = center + new Vector3Int(1, 0, 0);
            neighbors[2] = center + new Vector3Int(0, -1, 0);
            neighbors[3] = center + new Vector3Int(-1, -1, 0);
            neighbors[4] = center + new Vector3Int(-1, 0, 0);
            neighbors[5] = center + new Vector3Int(-1, 1, 0);
        } else {
            neighbors[0] = center + new Vector3Int(1, 1, 0);
            neighbors[1] = center + new Vector3Int(1, 0, 0);
            neighbors[2] = center + new Vector3Int(1, -1, 0);
            neighbors[3] = center + new Vector3Int(0, -1, 0);
            neighbors[4] = center + new Vector3Int(-1, 0, 0);
            neighbors[5] = center + new Vector3Int(0, 1, 0);
        }

        return neighbors[index];
    }

    public SmartTile GetRandomNeighbor(Vector3Int center) {
        int i = Random.Range(0, 6);
        return GetHexNeighbors(center)[i];
    }

    public List<SmartTile> GetNeighborsWithType(Vector3Int center, TileType type) {
        SmartTile[] neighbors = GetHexNeighbors(center);
        List<SmartTile> neighborsList = new();
        for (int i = 0; i < neighbors.Length; i++)
            if (neighbors[i].type == type)
                neighborsList.Add(neighbors[i]);

        return neighborsList;
    }

    public List<SmartTile> GetNeighborsWithType(Vector3Int center, TileType type1, TileType type2) {
        SmartTile[] neighbors = GetHexNeighbors(center);
        List<SmartTile> neighborsList = new();
        for (int i = 0; i < neighbors.Length; i++)
            if (neighbors[i].type == type1 || neighbors[i].type == type2)
                neighborsList.Add(neighbors[i]);

        return neighborsList;
    }

    public SmartTile GetTile(Vector3Int center) {
        return _tiles[center];
    }

    public SmartTile GetPlayerTile() {
        return _tiles[Playercoord];
    }

    public Vector3Int GetBuildingCoordinatesByPart(Vector3Int coord) {
        Vector3Int[] neighbors = GetHexCoordinates(coord);

        TileData data = TilesTable.TileByType(_tiles[coord].type);
        if (data.IsBuilding)
            switch (data.TIndex) {
                case 0:
                    return coord;

                case 1:
                    return neighbors[2];

                case 2:
                    return neighbors[3];

                case 3:
                    return neighbors[4];
            }

        UnityEngine.Debug.Log("Trouble");
        return Vector3Int.zero;
    }

    public SmartTile GetBuildingByPart(Vector3Int coord) {
        return _tiles[GetBuildingCoordinatesByPart(coord)];
    }
}