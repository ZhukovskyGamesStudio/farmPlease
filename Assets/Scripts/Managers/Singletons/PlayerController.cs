using System;
using System.Collections;
using Managers;
using Tables;
using UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

public class PlayerController : Singleton<PlayerController> {
    public static bool CanInteract = true;

    public KeyCode useToolKey;

    [HideInInspector] public Crop seedBagCrop;

    [HideInInspector] public SpriteRenderer currentBuildingSprite;

    [HideInInspector] public bool isBuilding;

    public Tile[] BuildingTiles;

    [Header("Building")] private GameObject _buildingPanel;

    private BuildingShopView _buildingShopView;

    private Tilemap _buildingTilemap;
    private BuildingType _currentBuilding;
    private Tile _currentTile;

    private Tool _curTool;
    private bool _fromShop;

    private GraphicRaycaster _graphicRaycaster;
    private Vector2Int _newBuildingcoord, _oldBuilddingscoord, _helpBuildingsCoord;

    private SmartTilemap _smartTilemap;
    
    private UIHud _uiHud;
    protected override bool IsDontDestroyOnLoad => false;

    private void Update() {
        if (CanInteract) {
            _graphicRaycaster.enabled = true;

            if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                int curIndex = (int) _curTool;
                curIndex++;
                curIndex %= 4;
                ChangeTool(curIndex);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                int curIndex = (int) _curTool;
                curIndex--;
                if (curIndex < 0)
                    curIndex = 3;
                ChangeTool(curIndex);
            }

            if (Input.GetKeyDown(KeyCode.Space) && GameModeManager.Instance.GameMode != GameMode.RealTime &&
                CanInteract) {
                Clock.Instance.TryAddDay();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                _uiHud.ClosePanel();

            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChangeTool(0);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                ChangeTool(1);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                ChangeTool(2);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                ChangeTool(3);
        } else {
            _graphicRaycaster.enabled = false;
        }

        if (isBuilding && _currentTile != null) {
            Vector3Int tmp = _buildingTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if ((Vector2Int)tmp != _newBuildingcoord) {
                _buildingTilemap.ClearAllTiles();
                _buildingTilemap.SetTile(tmp, _currentTile);

                _newBuildingcoord = (Vector2Int)tmp;
            }
        }
    }

    public void Init() {
        _uiHud = UIHud.Instance;
        _graphicRaycaster = _uiHud.GraphicRaycaster;
        _smartTilemap = SmartTilemap.Instance;
        _buildingShopView = _uiHud.ShopsPanel.BuildingShopView;
        _buildingPanel = _uiHud.BuildingPanel;
        _buildingTilemap = _smartTilemap.BuildingTilemap;

        ChangeTool(0);
        isBuilding = false;
        _oldBuilddingscoord = new Vector2Int(1000, 1000);
    }

     

    public void Click() {
        if (isBuilding) {
            if (_currentTile) {
                if (_smartTilemap.BuildingCanBePlaced(_currentBuilding, _smartTilemap.Playercoord) &&
                    _smartTilemap.Playercoord != _oldBuilddingscoord) {
                    _currentTile = null;
                    _buildingTilemap.ClearAllTiles();
                    if (!_fromShop) {
                        _smartTilemap.ActiveBuilding(_currentBuilding, _oldBuilddingscoord);
                        _smartTilemap.RemoveBuilding(_currentBuilding, _oldBuilddingscoord);
                    } else if (_currentBuilding == BuildingType.SprinklerTarget) {
                        _buildingShopView.BuyBuildingButton(BuildingType.Sprinkler);
                    } else if (_currentBuilding != BuildingType.Sprinkler) {
                        _buildingShopView.BuyBuildingButton(_currentBuilding);
                    }

                    _smartTilemap.PlaceBuilding(_currentBuilding, _smartTilemap.Playercoord);

                    SaveLoadManager.Instance.Sequence(false);

                    if (_fromShop)
                        if (_currentBuilding == BuildingType.Sprinkler) {
                            _helpBuildingsCoord = _smartTilemap.Playercoord;
                            InitializeBuilding(BuildingType.SprinklerTarget, 1);
                        } else {
                            StartStopBuilding();
                        }
                }
            } else if (_smartTilemap.AvailabilityCheck("building")) {
                switch (_smartTilemap.GetPlayerTile().type) {
                    case TileType.BiogenEmpty:
                    case TileType.BiogenFull:
                    case TileType.BiogenT1:
                    case TileType.BiogenT2:
                    case TileType.BiogenT3:
                    case TileType.BiogenConstruction:
                        _oldBuilddingscoord = _smartTilemap.GetBuildingCoordinatesByPart(_smartTilemap.Playercoord);

                        _smartTilemap.DeactiveBuilding(BuildingType.Biogen, _oldBuilddingscoord);
                        InitializeBuilding(BuildingType.Biogen);
                        break;

                    case TileType.FreshenerConstruction:
                    case TileType.FreshenerEmpty:
                    case TileType.FreshenerFull:
                    case TileType.FreshenerT1:
                    case TileType.FreshenerT2:
                    case TileType.FreshenerT3:
                    case TileType.Freshener1:
                    case TileType.Freshener2:
                    case TileType.Freshener3:
                    case TileType.Freshener4:
                    case TileType.Freshener5:
                    case TileType.Freshener6:
                        _oldBuilddingscoord = _smartTilemap.GetBuildingCoordinatesByPart(_smartTilemap.Playercoord);

                        _smartTilemap.DeactiveBuilding(BuildingType.Freshener, _oldBuilddingscoord);
                        InitializeBuilding(BuildingType.Freshener);
                        break;

                    case TileType.SprinklerConstruction:
                    case TileType.SprinklerEmpty:
                    case TileType.Sprinkler1:
                    case TileType.Sprinkler2:
                    case TileType.Sprinkler3:
                    case TileType.Sprinkler4:
                    case TileType.Sprinkler5:
                    case TileType.SprinklerFull:
                    case TileType.SprinklerT1:
                    case TileType.SprinklerT2:
                    case TileType.SprinklerT3:
                        _oldBuilddingscoord = _smartTilemap.GetBuildingCoordinatesByPart(_smartTilemap.Playercoord);

                        _smartTilemap.DeactiveBuilding(BuildingType.Sprinkler, _oldBuilddingscoord);
                        InitializeBuilding(BuildingType.Sprinkler);
                        break;

                    case TileType.SprinklerTarget:
                        _oldBuilddingscoord = _smartTilemap.Playercoord;

                        _smartTilemap.DeactiveBuilding(BuildingType.SprinklerTarget, _oldBuilddingscoord);
                        InitializeBuilding(BuildingType.SprinklerTarget);
                        break;

                    case TileType.SeedDoublerConstruction:
                    case TileType.SeedDoublerEmpty:
                    case TileType.SeedDoublerFull:
                    case TileType.SeedDoublerT1:
                    case TileType.SeedDoublerT2:
                    case TileType.SeedDoublerT3:
                        _oldBuilddingscoord = _smartTilemap.GetBuildingCoordinatesByPart(_smartTilemap.Playercoord);

                        _smartTilemap.DeactiveBuilding(BuildingType.SeedDoubler, _oldBuilddingscoord);
                        InitializeBuilding(BuildingType.SeedDoubler);
                        break;

                    case TileType.TractorConstruction:
                    case TileType.Tractor1:
                    case TileType.Tractor2:
                    case TileType.TractorT1:
                    case TileType.TractorT2:
                    case TileType.TractorT3:
                        _oldBuilddingscoord = _smartTilemap.GetBuildingCoordinatesByPart(_smartTilemap.Playercoord);

                        _smartTilemap.DeactiveBuilding(BuildingType.Tractor, _oldBuilddingscoord);
                        InitializeBuilding(BuildingType.Tractor);
                        break;
                }
            }
        } else {
            if (CanInteract)
                StartCoroutine(ClickCoroutine());
        }
    }

    public IEnumerator ClickCoroutine() {
        SaveLoadManager.Instance.Sequence(true);

        if (GameModeManager.Instance.ShowTileType)
            UnityEngine.Debug.Log(_smartTilemap.Playercoord + "   " + _smartTilemap.GetTile(_smartTilemap.Playercoord).type);

        if (_smartTilemap.AvailabilityCheck("click"))
            yield return StartCoroutine(_smartTilemap.ClickTile());
        else
            switch (_curTool) {
                case Tool.Hoe:
                    if (Energy.Instance.HasEnergy())
                        if (_smartTilemap.AvailabilityCheck("hoe")) {
                         
                            Energy.Instance.LoseOneEnergy();
                            Vector2Int coord = _smartTilemap.Playercoord;
                            InventoryManager.Instance.AddXp(1);
                            yield return StartCoroutine(_smartTilemap.HoeTile());

                            if (InventoryManager.Instance.IsToolWorking(ToolBuff.Doublehoe))
                                yield return StartCoroutine(_smartTilemap.HoeRandomNeighbor(coord));
                        }

                    break;

                case Tool.Watercan:
                    bool hasUnlimitedWatercan = InventoryManager.Instance.IsToolWorking(ToolBuff.Unlimitedwatercan);
                    if (Energy.Instance.HasEnergy(!hasUnlimitedWatercan) || hasUnlimitedWatercan)
                        if (_smartTilemap.AvailabilityCheck("water")) {
                            if (!InventoryManager.Instance.IsToolWorking(ToolBuff.Unlimitedwatercan)) {
                                Energy.Instance.LoseOneEnergy();
                            }
                            InventoryManager.Instance.AddXp(1);
                            yield return StartCoroutine(_smartTilemap.WaterTile());
                        }

                    break;

                case Tool.SeedBag:
                    if (Energy.Instance.HasEnergy() || InventoryManager.Instance.IsToolWorking(ToolBuff.Carpetseeder))
                        if (InventoryManager.SeedsInventory[seedBagCrop] > 0)
                            if (_smartTilemap.AvailabilityCheck("seed")) {
                                InventoryManager.Instance.LoseSeed(seedBagCrop);
                                if (!InventoryManager.Instance.IsToolWorking(ToolBuff.Carpetseeder)) {
                                    Energy.Instance.LoseOneEnergy();
                                }
                                InventoryManager.Instance.AddXp(1);
                                yield return StartCoroutine(_smartTilemap.SeedTile(seedBagCrop));
                            }

                    break;

                case Tool.Collect:
                    if (InventoryManager.Instance.IsToolWorking(ToolBuff.Wetscythe) &&
                        _smartTilemap.AvailabilityCheck("water")) {
                        InventoryManager.Instance.AddXp(1);
                        yield return StartCoroutine(_smartTilemap.WaterTile());
                    }

                    if (InventoryManager.Instance.IsToolWorking(ToolBuff.Greenscythe) &&
                        _smartTilemap.GetPlayerTile().type == TileType.WateredSoil) {
                        if (Energy.Instance.HasEnergy()) {
                            Vector2Int coord = _smartTilemap.Playercoord;
                            while (_smartTilemap.GetTile(coord).type == TileType.WateredSoil)
                                yield return StartCoroutine(_smartTilemap.GetTile(coord)
                                    .OnNeyDayed(_smartTilemap.animtime));
                        }
                    } else if (_smartTilemap.AvailabilityCheck("collect")) {
                        InventoryManager.Instance.AddXp(1);
                        yield return StartCoroutine(_smartTilemap.CollectTile());
                    }

                    break;
            }

        yield return false;
        SaveLoadManager.Instance.Sequence(false);
    }

    public void RightClick() {
        if (isBuilding) {
            if (_currentTile) {
                _currentTile = null;
                _buildingTilemap.ClearAllTiles();

                if (!_fromShop)
                    _smartTilemap.ActiveBuilding(_currentBuilding, _oldBuilddingscoord);
                else if (_currentBuilding == BuildingType.SprinklerTarget)
                    _smartTilemap.RemoveBuilding(BuildingType.Sprinkler, _helpBuildingsCoord);

                SaveLoadManager.Instance.Sequence(false);

                if (_fromShop)
                    StartStopBuilding();
            } else {
                StartStopBuilding();
            }
        } else if (CanInteract) {
            StartCoroutine(RightClickCoroutine());
        }
    }

    public IEnumerator RightClickCoroutine() {
        Tool before = _curTool;
        _curTool = Tool.Collect;
        yield return StartCoroutine(ClickCoroutine());
        _curTool = before;
    }

    public Tool CurTool => _curTool;
    public void ChangeTool(int index) {
        _curTool = (Tool) index;
        _uiHud.ChangeInventoryHover(index);
    }

    public void ChangeTool(Tool tool) {
        ChangeTool((int) tool);
    }

    public void StartStopBuilding() {
        if (isBuilding) {
            _uiHud.SetBuildingPanelState(false);
            isBuilding = false;
        } else {
            _uiHud.SetBuildingPanelState(true);
            _buildingTilemap.ClearAllTiles();
            isBuilding = true;
        }
    }

    public void InitializeBuilding(BuildingType type, int price = 0) {
        _currentBuilding = type;
        _fromShop = price > 0;

        _currentTile = BuildingsTable.BuildingByType(type).BuildingPanelTile;
    }

   
}

[Serializable]
public enum Tool {
    Hoe = 0,
    Watercan,
    SeedBag,
    Collect
}