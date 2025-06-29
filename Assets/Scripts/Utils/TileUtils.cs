using System.Collections.Generic;
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
}