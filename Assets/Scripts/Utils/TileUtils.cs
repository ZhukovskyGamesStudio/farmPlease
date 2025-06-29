using System.Collections.Generic;
using System.Linq;
using Managers;
using Tables;
using UnityEngine;

public static class TileUtils {
    public static void UnlockTilesInRadius(int radius) {
        UnlockTiles(SmartTilemap.GenerateInitialCircleTiles());
    }

    public static List<Vector2Int> GenerateCircleTiles(int radius) {
        int circle = 0;
        int step = 0;
        int i = 0;
        List<Vector2Int> res = new List<Vector2Int>();
        Vector2Int curCoord = new(0, 0);

        while (circle < radius) {
            res.Add(curCoord);
            curCoord = TilemapTools.Next(curCoord, circle, step);

            step++;
            if (step == circle * 6 || circle == 0) {
                circle++;
                step = 0;
            }

            if (i > 10000) {
                Debug.LogError("spent too much in while. Instant Break");
                break;
            }
        }

        return res;
    }
    
    public static void UnlockTiles(List<Vector2Int> coords) {
        TilesData tilesData = SaveLoadManager.CurrentSave.TilesData;
        foreach (Vector2Int coord in coords) {
            if (!tilesData.Tiles.ContainsKey(coord)) {
                tilesData.Tiles.Add(coord, TileType.Sand);
            } else if (tilesData.Tiles[coord] == TileType.Rocks) {
                tilesData.Tiles[coord] = TileType.Sand;
            }
        }

        SaveLoadManager.SaveGame();

        if (SmartTilemap.Instance) {
            SmartTilemap.Instance.GenerateTilesWithData(tilesData);
        }
    }

    public static int CountAvailableTiles() {
        return SaveLoadManager.CurrentSave.TilesData.Tiles.Values.Count(t=> !NotAvailableTiles.Contains(t))-1;
    }

    private static List<TileType> NotAvailableTiles => new List<TileType>() {
        TileType.None, TileType.Rocks,
        TileType.QuestBoard1_new, TileType.QuestBoard1_00, TileType.QuestBoard1_01, TileType.QuestBoard1_10, TileType.QuestBoard1_11,
        TileType.QuestBoard2, TileType.QuestBoard3, TileType.QuestBoard4
    };

    public static int CountTilesOfType(TileType type) {
        return SaveLoadManager.CurrentSave.TilesData.Tiles.Values.Count(t=>t ==type);
    }
}