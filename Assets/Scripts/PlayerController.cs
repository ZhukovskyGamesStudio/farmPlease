using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;
    public static bool canInteract = true;

    public KeyCode useToolKey;
    public int maxEnergy;
    public int curEnergy;

    [HideInInspector]
    public CropsType seedBagCrop;

    [HideInInspector]
    public SpriteRenderer currentBuildingSprite;

    [HideInInspector]
    public bool isBuilding;

    public Tile[] BuildingTiles;

    [Header("Building")]
    private GameObject BuildingPanel;

    private BuildingShopPanel BuildingShopPanel;

    private Tilemap BuildingTilemap;
    private BuildingType currentBuilding;
    private Tile currentTile;

    private Tool curTool;
    private bool FromShop;

    private GraphicRaycaster GraphicRaycaster;
    private Vector3Int newBuildingcoord, oldBuilddingscoord, helpBuildingsCoord;

    private SmartTilemap SmartTilemap;
    private UIScript UIScript;

    public void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Update() {
        if (canInteract) {
            GraphicRaycaster.enabled = true;

            if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                int curIndex = (int) curTool;
                curIndex++;
                curIndex %= 4;
                ChangeTool(curIndex);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                int curIndex = (int) curTool;
                curIndex--;
                if (curIndex < 0)
                    curIndex = 3;
                ChangeTool(curIndex);
            }

            if (Input.GetKeyDown(KeyCode.Space) && GameModeManager.instance.GameMode != GameMode.RealTime &&
                canInteract)
                TimeManager.instance.AddDay();

            if (Input.GetKeyDown(KeyCode.Escape))
                UIScript.ClosePanel();

            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChangeTool(0);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                ChangeTool(1);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                ChangeTool(2);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                ChangeTool(3);
        } else {
            GraphicRaycaster.enabled = false;
        }

        if (isBuilding && currentTile != null) {
            Vector3Int tmp = BuildingTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (tmp != newBuildingcoord) {
                BuildingTilemap.ClearAllTiles();
                BuildingTilemap.SetTile(tmp, currentTile);

                newBuildingcoord = tmp;
            }
        }
    }

    public void Init() {
        UIScript = UIScript.instance;
        GraphicRaycaster = UIScript.GraphicRaycaster;
        SmartTilemap = SmartTilemap.instance;
        BuildingShopPanel = UIScript.ShopsPanel.BuildingShopPanel;
        BuildingPanel = UIScript.BuildingPanel;
        BuildingTilemap = SmartTilemap.BuildingTilemap;

        ChangeTool(0);
        isBuilding = false;
        oldBuilddingscoord = new Vector3Int(1000, 1000, 1000);
    }

    /**********/

    public void LoseOneEnergy() {
        curEnergy--;
        if (curEnergy < 0)
            curEnergy = 0;

        if (GameModeManager.instance.InfiniteEnergy)
            curEnergy = maxEnergy;

        UIScript.ChangeBattery(curEnergy);
    }

    public void SetEnergy(int newEnergy, bool isRefill = false) {
        if (isRefill)
            curEnergy = maxEnergy;
        else
            curEnergy = newEnergy;
        UIScript.ChangeBattery(curEnergy);
    }

    public void RestoreEnergy() {
        curEnergy = maxEnergy;
        UIScript.ChangeBattery(curEnergy);
    }

    public void RestoreEnergy(int amount) {
        curEnergy += amount;
        if (curEnergy > maxEnergy)
            curEnergy = maxEnergy;
        UIScript.ChangeBattery(curEnergy);
    }

    public void Click() {
        if (isBuilding) {
            if (currentTile) {
                if (SmartTilemap.BuildingCanBePlaced(currentBuilding, SmartTilemap.Playercoord) &&
                    SmartTilemap.Playercoord != oldBuilddingscoord) {
                    currentTile = null;
                    BuildingTilemap.ClearAllTiles();
                    if (!FromShop) {
                        SmartTilemap.ActiveBuilding(currentBuilding, oldBuilddingscoord);
                        SmartTilemap.RemoveBuilding(currentBuilding, oldBuilddingscoord);
                    } else if (currentBuilding == BuildingType.Sprinkler_target) {
                        BuildingShopPanel.BuyBuildingButton(BuildingType.Sprinkler);
                    } else if (currentBuilding != BuildingType.Sprinkler) {
                        BuildingShopPanel.BuyBuildingButton(currentBuilding);
                    }

                    SmartTilemap.PlaceBuilding(currentBuilding, SmartTilemap.Playercoord);

                    SaveLoadManager.instance.Sequence(false);

                    if (FromShop)
                        if (currentBuilding == BuildingType.Sprinkler) {
                            helpBuildingsCoord = SmartTilemap.Playercoord;
                            InitializeBuilding(BuildingType.Sprinkler_target, 1);
                        } else {
                            StartStopBuilding();
                        }
                }
            } else if (SmartTilemap.AvailabilityCheck("building")) {
                switch (SmartTilemap.GetPlayerTile().type) {
                    case TileType.Biogen_empty:
                    case TileType.Biogen_full:
                    case TileType.Biogen_T1:
                    case TileType.Biogen_T2:
                    case TileType.Biogen_T3:
                    case TileType.Biogen_Construction:
                        oldBuilddingscoord = SmartTilemap.GetBuildingCoordinatesByPart(SmartTilemap.Playercoord);

                        SmartTilemap.DeactiveBuilding(BuildingType.Biogen, oldBuilddingscoord);
                        InitializeBuilding(BuildingType.Biogen);
                        break;

                    case TileType.Freshener_Construction:
                    case TileType.Freshener_empty:
                    case TileType.Freshener_full:
                    case TileType.Freshener_T1:
                    case TileType.Freshener_T2:
                    case TileType.Freshener_T3:
                    case TileType.Freshener_1:
                    case TileType.Freshener_2:
                    case TileType.Freshener_3:
                    case TileType.Freshener_4:
                    case TileType.Freshener_5:
                    case TileType.Freshener_6:
                        oldBuilddingscoord = SmartTilemap.GetBuildingCoordinatesByPart(SmartTilemap.Playercoord);

                        SmartTilemap.DeactiveBuilding(BuildingType.Freshener, oldBuilddingscoord);
                        InitializeBuilding(BuildingType.Freshener);
                        break;

                    case TileType.Sprinkler_Construction:
                    case TileType.Sprinkler_empty:
                    case TileType.Sprinkler_1:
                    case TileType.Sprinkler_2:
                    case TileType.Sprinkler_3:
                    case TileType.Sprinkler_4:
                    case TileType.Sprinkler_5:
                    case TileType.Sprinkler_full:
                    case TileType.Sprinkler_T1:
                    case TileType.Sprinkler_T2:
                    case TileType.Sprinkler_T3:
                        oldBuilddingscoord = SmartTilemap.GetBuildingCoordinatesByPart(SmartTilemap.Playercoord);

                        SmartTilemap.DeactiveBuilding(BuildingType.Sprinkler, oldBuilddingscoord);
                        InitializeBuilding(BuildingType.Sprinkler);
                        break;

                    case TileType.Sprinkler_target:
                        oldBuilddingscoord = SmartTilemap.Playercoord;

                        SmartTilemap.DeactiveBuilding(BuildingType.Sprinkler_target, oldBuilddingscoord);
                        InitializeBuilding(BuildingType.Sprinkler_target);
                        break;

                    case TileType.SeedDoubler_Construction:
                    case TileType.SeedDoubler_empty:
                    case TileType.SeedDoubler_full:
                    case TileType.SeedDoubler_T1:
                    case TileType.SeedDoubler_T2:
                    case TileType.SeedDoubler_T3:
                        oldBuilddingscoord = SmartTilemap.GetBuildingCoordinatesByPart(SmartTilemap.Playercoord);

                        SmartTilemap.DeactiveBuilding(BuildingType.SeedDoubler, oldBuilddingscoord);
                        InitializeBuilding(BuildingType.SeedDoubler);
                        break;

                    case TileType.Tractor_Construction:
                    case TileType.Tractor_1:
                    case TileType.Tractor_2:
                    case TileType.Tractor_T1:
                    case TileType.Tractor_T2:
                    case TileType.Tractor_T3:
                        oldBuilddingscoord = SmartTilemap.GetBuildingCoordinatesByPart(SmartTilemap.Playercoord);

                        SmartTilemap.DeactiveBuilding(BuildingType.Tractor, oldBuilddingscoord);
                        InitializeBuilding(BuildingType.Tractor);
                        break;
                }
            }
        } else {
            if (canInteract)
                StartCoroutine(ClickCoroutine());
        }
    }

    public bool HasEnergy() {
        if (curEnergy == 0) {
            UIScript.NoEnergy();
            AudioManager.instance.PlaySound(Sounds.ZeroEnergy);
        }

        return curEnergy > 0;
    }

    public IEnumerator ClickCoroutine() {
        SaveLoadManager.instance.Sequence(true);

        if (GameModeManager.instance.ShowTileType)
            Debug.Log(SmartTilemap.Playercoord + "   " + SmartTilemap.GetTile(SmartTilemap.Playercoord).type);

        if (SmartTilemap.AvailabilityCheck("click"))
            yield return StartCoroutine(SmartTilemap.ClickTile());
        else
            switch (curTool) {
                case Tool.Hoe:
                    if (HasEnergy())
                        if (SmartTilemap.AvailabilityCheck("hoe")) {
                            LoseOneEnergy();
                            Vector3Int coord = SmartTilemap.Playercoord;
                            yield return StartCoroutine(SmartTilemap.HoeTile());

                            if (InventoryManager.instance.IsToolWorking(ToolType.Doublehoe))
                                yield return StartCoroutine(SmartTilemap.HoeRandomNeighbor(coord));
                        }

                    break;

                case Tool.Watercan:
                    if (HasEnergy() || InventoryManager.instance.IsToolWorking(ToolType.Unlimitedwatercan))
                        if (SmartTilemap.AvailabilityCheck("water")) {
                            if (!InventoryManager.instance.IsToolWorking(ToolType.Unlimitedwatercan))
                                LoseOneEnergy();
                            yield return StartCoroutine(SmartTilemap.WaterTile());
                        }

                    break;

                case Tool.SeedBag:
                    if (HasEnergy() || InventoryManager.instance.IsToolWorking(ToolType.Carpetseeder))
                        if (InventoryManager.instance.seedsInventory[seedBagCrop] > 0)
                            if (SmartTilemap.AvailabilityCheck("seed")) {
                                InventoryManager.instance.LoseSeed(seedBagCrop);
                                if (!InventoryManager.instance.IsToolWorking(ToolType.Carpetseeder))
                                    LoseOneEnergy();
                                yield return StartCoroutine(SmartTilemap.SeedTile(seedBagCrop));
                            }

                    break;

                case Tool.Collect:
                    if (InventoryManager.instance.IsToolWorking(ToolType.Wetscythe) &&
                        SmartTilemap.AvailabilityCheck("water"))
                        yield return StartCoroutine(SmartTilemap.WaterTile());

                    if (InventoryManager.instance.IsToolWorking(ToolType.Greenscythe) &&
                        SmartTilemap.GetPlayerTile().type == TileType.WateredSoil) {
                        if (HasEnergy()) {
                            Vector3Int coord = SmartTilemap.Playercoord;
                            while (SmartTilemap.GetTile(coord).type == TileType.WateredSoil)
                                yield return StartCoroutine(SmartTilemap.GetTile(coord)
                                    .OnNeyDayed(SmartTilemap.animtime));
                        }
                    } else if (SmartTilemap.AvailabilityCheck("collect")) {
                        yield return StartCoroutine(SmartTilemap.CollectTile());
                    }

                    break;
            }

        yield return false;
        SaveLoadManager.instance.Sequence(false);
    }

    public void RightClick() {
        if (isBuilding) {
            if (currentTile) {
                currentTile = null;
                BuildingTilemap.ClearAllTiles();

                if (!FromShop)
                    SmartTilemap.ActiveBuilding(currentBuilding, oldBuilddingscoord);
                else if (currentBuilding == BuildingType.Sprinkler_target)
                    SmartTilemap.RemoveBuilding(BuildingType.Sprinkler, helpBuildingsCoord);

                SaveLoadManager.instance.Sequence(false);

                if (FromShop)
                    StartStopBuilding();
            } else {
                StartStopBuilding();
            }
        } else if (canInteract) {
            StartCoroutine(RightClickCoroutine());
        }
    }

    public IEnumerator RightClickCoroutine() {
        Tool before = curTool;
        curTool = Tool.Collect;
        yield return StartCoroutine(ClickCoroutine());
        curTool = before;
    }

    public void ChangeTool(int index) {
        curTool = (Tool) index;
        UIScript.ChangeInventoryHover(index);
    }

    public void StartStopBuilding() {
        if (isBuilding) {
            BuildingPanel.SetActive(false);
            isBuilding = false;
        } else {
            BuildingPanel.SetActive(true);

            BuildingTilemap.ClearAllTiles();
            isBuilding = true;
        }

        UIScript.TimePanel.gameObject.SetActive(!isBuilding);
        UIScript.ShopsPanel.gameObject.SetActive(!isBuilding);

        for (int i = 0; i < UIScript.FastPanelScript.SlotsImages.Length; i++)
            UIScript.FastPanelScript.SlotsImages[i].gameObject.SetActive(!isBuilding);

        UIScript.CroponomButton.gameObject.SetActive(!isBuilding);
        UIScript.Backpack.gameObject.SetActive(!isBuilding);
        UIScript.BatteryScript.gameObject.SetActive(!isBuilding);
    }

    public void InitializeBuilding(BuildingType type, int price = 0) {
        currentBuilding = type;
        FromShop = price > 0;

        currentTile = BuildingsTable.BuildingByType(type).BuildingPanelTile;
    }

    [Serializable]
    private enum Tool {
        Hoe = 0,
        Watercan,
        SeedBag,
        Collect
    }
}