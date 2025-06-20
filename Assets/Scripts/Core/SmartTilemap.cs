using System.Collections;
using System.Collections.Generic;
using Managers;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = Managers.Debug;
using TileData = Tables.TileData;

public class SmartTilemap : MonoBehaviour {
    [Resettable]
    public static SmartTilemap Instance;
    public Tilemap MainTilemap;
    public Tilemap BuildingTilemap;
    public ToolsAnimTilemap toolsAnimTilemap;

    public TilesTable tilesTablePrefab;
    public Transform TilesHolder;
    public Vector2Int Playercoord;

    public float animtime = 0.5f;
    private const int TILES_RADIUS = 11;
    private const int STARTING_CIRCLE_RADIUS = 5;
    private Vector2Int _fieldSizeI = new(-11, 9);
    private Vector2Int _fieldSizeJ = new(-13, 13);
    private Dictionary<Vector2Int, SmartTile> _tiles;
    private Camera _mainCamera;

    public void Awake() {
        if (Instance == null) {
            Instance = this;
            _mainCamera = Camera.main;
        } else if (Instance != this)
            Destroy(gameObject);
    }

    public void Update() {
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x > 1000000) {
            //wraps unimportant editor bug Screen position out of view frustum (screen pos inf, -inf, 0.000000) 
            mousePos = Vector3.zero;
        }

        Playercoord = (Vector2Int)MainTilemap.WorldToCell(_mainCamera.ScreenToWorldPoint(mousePos));
       // UnityEngine.Debug.Log("Playercoord: " + Playercoord);
    }

    public static void UnlockTiles(TilesData tilesData) {
        foreach (var coord in tilesData.Tiles.Keys) {
            if(!SaveLoadManager.CurrentSave.TilesData.Tiles.ContainsKey(coord)) {
                SaveLoadManager.CurrentSave.TilesData.Tiles.Add(coord, TileType.Sand);
            } else if(SaveLoadManager.CurrentSave.TilesData.Tiles[coord] == TileType.Rocks) {
                SaveLoadManager.CurrentSave.TilesData.Tiles[coord] = TileType.Sand;
            }
        }
    }

    public static TilesData GenerateFtueTiles() {
        var tilesData = GenerateCircleTiles(TILES_RADIUS, TileType.Rocks);
        List<Vector2Int> ftueTiles = new() {
            new Vector2Int(-2, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(-2, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        };
        foreach (var VARIABLE in ftueTiles) {
            tilesData.Tiles[VARIABLE] = TileType.Sand;
           // tilesData.Tiles.Add(VARIABLE, TileType.Sand);
        }

        return tilesData;
    }
    
    public static TilesData GenerateInitialCircleTiles() {
        var tilesData = GenerateCircleTiles(STARTING_CIRCLE_RADIUS, TileType.Sand);
        return tilesData;
    }

    public static TilesData GenerateCircleTiles(int radius, TileType type) {
        var tilesData = new TilesData();

        int circle = 0;
        int step = 0;
        int i = 0;
        Vector2Int curCoord = new(0, 0);

        while (circle < radius) {
           // TileType type = circle < STARTING_CIRCLE_RADIUS ? TileType.Sand : TileType.Rocks;
            tilesData.Tiles.Add(curCoord, type);

            curCoord = TilemapTools.Next(curCoord, circle, step);

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

        return tilesData;
    }

    public void GenerateTilesWithData(TilesData data) {
        MainTilemap.ClearAllTiles();
        if(_tiles != null) {
            foreach (var VARIABLE in _tiles) {
                Destroy(VARIABLE.Value.gameObject);
            }
        }
        _tiles = new Dictionary<Vector2Int, SmartTile>();
        foreach (var pos in data.Tiles.Keys) {
            TileType tile = data.Tiles[pos];
            Vector2Int position = new(pos.x, pos.y);
            GameObject tileObject = new();
            tileObject.transform.parent = TilesHolder;
            SmartTile smarttile = tileObject.AddComponent<SmartTile>();

            smarttile.Init(this, tile, position);
            MainTilemap.SetTile((Vector3Int)position, TilesTable.TileByType(tile).TileBase);

            _tiles.Add(position, smarttile);
        }
    }

    public IEnumerator NewDay(HappeningType type) {
        SetHappeningType(type);
        string sequenceId = SaveLoadManager.Instance.StartSequence();
        Dictionary<Vector2Int, SmartTile> tempTiles = new(_tiles);
        List<SmartTile> toNewDay = new();

        foreach (KeyValuePair<Vector2Int, SmartTile> smartTile in tempTiles)
            if (smartTile.Value.CanbeNewDayed())
                toNewDay.Add(smartTile.Value);
        for (int i = 0; i < toNewDay.Count; i++) yield return StartCoroutine(toNewDay[i].OnNeyDayed(animtime / 5));
        yield return StartCoroutine(HappeningSequence());
        SaveLoadManager.Instance.EndSequence(sequenceId);
    }

    private HappeningType _happeningType;

    public void SetHappeningType(HappeningType happeningType) {
        _happeningType = happeningType;
    }

    public IEnumerator HappeningSequence() {
        string sequenceId = SaveLoadManager.Instance.StartSequence();
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

        SaveLoadManager.Instance.EndSequence(sequenceId);
    }

    public void PlaceTile(Vector2Int coord, TileType type) {
        MainTilemap.SetTile((Vector3Int)coord, TilesTable.TileByType(type).TileBase);
        SaveLoadManager.CurrentSave.TilesData.Tiles[coord] = type;
    }

    public bool BuildingCanBePlaced(BuildingType type, Vector2Int coord) {
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

    public void PlaceBuilding(BuildingType type, Vector2Int coord) {
        SmartTile[] neighbors = GetHexNeighbors(coord);
        switch (type) {
            case BuildingType.Biogen:
                _tiles[coord].SwitchType(TileType.BiogenConstruction);
                neighbors[5].SwitchType(TileType.BiogenT1);
                neighbors[0].SwitchType(TileType.BiogenT2);
                neighbors[1].SwitchType(TileType.BiogenT3);
                InventoryManager.Instance.RemoveBuilding(BuildingType.Biogen);
                break;

            case BuildingType.Freshener:
                _tiles[coord].SwitchType(TileType.FreshenerConstruction);
                neighbors[5].SwitchType(TileType.FreshenerT1);
                neighbors[0].SwitchType(TileType.FreshenerT2);
                neighbors[1].SwitchType(TileType.FreshenerT3);
                InventoryManager.Instance.RemoveBuilding(BuildingType.Freshener);
                break;

            case BuildingType.Sprinkler:
                _tiles[coord].SwitchType(TileType.SprinklerConstruction);
                neighbors[5].SwitchType(TileType.SprinklerT1);
                neighbors[0].SwitchType(TileType.SprinklerT2);
                neighbors[1].SwitchType(TileType.SprinklerT3);
                InventoryManager.Instance.RemoveBuilding(BuildingType.Sprinkler);
                break;

            case BuildingType.SprinklerTarget:
                _tiles[coord].SwitchType(TileType.SprinklerTarget);
                break;

            case BuildingType.SeedDoubler:
                _tiles[coord].SwitchType(TileType.SeedDoublerConstruction);
                neighbors[5].SwitchType(TileType.SeedDoublerT1);
                neighbors[0].SwitchType(TileType.SeedDoublerT2);
                neighbors[1].SwitchType(TileType.SeedDoublerT3);
                SaveLoadManager.CurrentSave.SeedShopData.AmbarCrop = Crop.None;
                InventoryManager.Instance.RemoveBuilding(BuildingType.SeedDoubler);
                break;

            case BuildingType.Tractor:
                _tiles[coord].SwitchType(TileType.TractorConstruction);
                neighbors[5].SwitchType(TileType.TractorT1);
                neighbors[0].SwitchType(TileType.TractorT2);
                neighbors[1].SwitchType(TileType.TractorT3);
                InventoryManager.Instance.RemoveBuilding(BuildingType.Tractor);
                break;
            case BuildingType.QuestBoard:
                _tiles[coord].SwitchType(TileType.QuestBoard1_new);
                //TODO add other tiles
                break;
        }
    }

    public void DeactiveBuilding(BuildingType type, Vector2Int coord) {
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

    public void ActiveBuilding(BuildingType type, Vector2Int coord) {
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

    public void RemoveBuilding(BuildingType type, Vector2Int coord) {
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
        bool hasGoldenScythe = RealShopUtils.IsGoldenScytheActive(SaveLoadManager.CurrentSave.RealShopData);
        yield return StartCoroutine(
            _tiles[Playercoord].OnCollected(InventoryManager.Instance.IsToolWorking(ToolBuff.Greenscythe),hasGoldenScythe, animtime / 3));
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

    public IEnumerator HoeRandomNeighbor(Vector2Int center) {
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
        Dictionary<Vector2Int, SmartTile> tempTiles = new(_tiles);
        List<SmartTile> toErosion = new();
        foreach (KeyValuePair<Vector2Int, SmartTile> smartTile in tempTiles)
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
        Dictionary<Vector2Int, SmartTile> tempTiles = new(_tiles);
        foreach (KeyValuePair<Vector2Int, SmartTile> smartTile in tempTiles)
            if (smartTile.Value.CanBeWatered())
                yield return StartCoroutine(smartTile.Value.OnWatered(animtime / 5f, true));
    }

    public IEnumerator Insects() {
        List<Vector2Int> flycatcherPosition = new();
        foreach (KeyValuePair<Vector2Int, SmartTile> smartTile in _tiles)
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
            Dictionary<Vector2Int, SmartTile> tempTiles = new(_tiles);
            foreach (KeyValuePair<Vector2Int, SmartTile> smartTile in tempTiles)
                if (smartTile.Value.CanBeCollected())
                    yield return StartCoroutine(smartTile.Value.OnInsected(animtime / 5));
        }
    }

    public SmartTile[] GetAllTiles() {
        SmartTile[] resTiles = new SmartTile[_tiles.Count];

        int i = 0;
        foreach (KeyValuePair<Vector2Int, SmartTile> tile in _tiles) {
            resTiles[i] = tile.Value;
            i++;
        }

        return resTiles;
    }

    public SmartTile[] GetAllTiles(TileType type) {
        List<SmartTile> tilesL = new();

        foreach (KeyValuePair<Vector2Int, SmartTile> tile in _tiles)
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

    public SmartTile[] GetHexNeighbors(Vector2Int center) {
        SmartTile[] neighbors = new SmartTile[6];

        if (center.y % 2 == 0) {
            _tiles.TryGetValue(center + new Vector2Int(0, 1),out neighbors[0]);
            _tiles.TryGetValue(center + new Vector2Int(1, 0),out neighbors[1]);
            _tiles.TryGetValue(center + new Vector2Int(0, -1),out neighbors[2]);
            _tiles.TryGetValue(center + new Vector2Int(-1, -1),out neighbors[3]);
            _tiles.TryGetValue(center + new Vector2Int(-1, 0),out neighbors[4]);
            _tiles.TryGetValue(center + new Vector2Int(-1, 1),out neighbors[5]);
        } else {
             _tiles.TryGetValue(center + new Vector2Int(1, 1),out neighbors[0]);
             _tiles.TryGetValue(center + new Vector2Int(1, 0),out neighbors[1]);
             _tiles.TryGetValue(center + new Vector2Int(1, -1),out neighbors[2]);
             _tiles.TryGetValue(center + new Vector2Int(0, -1),out neighbors[3]);
             _tiles.TryGetValue(center + new Vector2Int(-1, 0),out neighbors[4]);
             _tiles.TryGetValue(center + new Vector2Int(0, 1),out neighbors[5]);
        }

        return neighbors;
    }

    public Vector2Int[] GetHexCoordinates(Vector2Int center) {
        Vector2Int[] neighbors = new Vector2Int[6];

        if (center.y % 2 == 0) {
            neighbors[0] = center + new Vector2Int(0, 1);
            neighbors[1] = center + new Vector2Int(1, 0);
            neighbors[2] = center + new Vector2Int(0, -1);
            neighbors[3] = center + new Vector2Int(-1, -1);
            neighbors[4] = center + new Vector2Int(-1, 0);
            neighbors[5] = center + new Vector2Int(-1, 1);
        } else {
            neighbors[0] = center + new Vector2Int(1, 1);
            neighbors[1] = center + new Vector2Int(1, 0);
            neighbors[2] = center + new Vector2Int(1, -1);
            neighbors[3] = center + new Vector2Int(0, -1);
            neighbors[4] = center + new Vector2Int(-1, 0);
            neighbors[5] = center + new Vector2Int(0, 1);
        }

        return neighbors;
    }

    public SmartTile GetRandomNeighbor(Vector2Int center) {
        int i = Random.Range(0, 6);
        return GetHexNeighbors(center)[i];
    }

    public List<SmartTile> GetNeighborsWithType(Vector2Int center, TileType type) {
        SmartTile[] neighbors = GetHexNeighbors(center);
        List<SmartTile> neighborsList = new();
        for (int i = 0; i < neighbors.Length; i++)
            if (neighbors[i].type == type)
                neighborsList.Add(neighbors[i]);

        return neighborsList;
    }

    public List<SmartTile> GetNeighborsWithType(Vector2Int center, TileType type1, TileType type2) {
        SmartTile[] neighbors = GetHexNeighbors(center);
        List<SmartTile> neighborsList = new();
        for (int i = 0; i < neighbors.Length; i++)
            if (neighbors[i].type == type1 || neighbors[i].type == type2)
                neighborsList.Add(neighbors[i]);

        return neighborsList;
    }

    public SmartTile GetTile(Vector2Int center) {
        return _tiles[center];
    }

    public SmartTile GetPlayerTile() {
        return _tiles[Playercoord];
    }

    public Vector2Int GetBuildingCoordinatesByPart(Vector2Int coord) {
        Vector2Int[] neighbors = GetHexCoordinates(coord);

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
        return Vector2Int.zero;
    }

    public SmartTile GetBuildingByPart(Vector2Int coord) {
        return _tiles[GetBuildingCoordinatesByPart(coord)];
    }
}