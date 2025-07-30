using System;
using UnityEngine;
using ZhukovskyGamesPlugin;

[Serializable]
public class TilesData {
    public SerializableDictionary<Vector2Int, TileType> Tiles = new SerializableDictionary<Vector2Int, TileType>();
}
