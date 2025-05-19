using UnityEngine;

public static class TilemapTools {
    public static Vector2Int GetHexNeighbor(Vector2Int center, int index) {
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

        return neighbors[index];
    }

    public static Vector2Int Next(Vector2Int now, int circle, int step) {
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

        return now + new Vector2Int(0, 0);
    }
}