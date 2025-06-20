using System.Linq;
using Tables;
using UnityEngine;

public static class MigrationUtils {
    //Obsolete if all saves BuildingPrice == -1
    public static void TryMigrateToBuildingShopData(GameSaveProfile save) {
        if (save.BuildingShopData == null) {
            save.BuildingShopData = new BuildingShopData();
        }

        if (save.BuildingPrice >= 0) {
            save.BuildingShopData.BuildingPriceIndex = save.BuildingPrice;
            save.BuildingPrice = -1;
        }
    }

    public static void TryMigrateToQuestsData(GameSaveProfile save) {
       /* var pos = new Vector2Int(-2, 3);

        if (save.TilesData.Tiles.ContainsKey(pos)) {
            save.TilesData.Tiles[pos] = TileType.QuestBoard1;
        } else {
            save.TilesData.Tiles.Add(pos, TileType.QuestBoard1);
        }*/
       
        /*
        if (save.TilesData.Tiles.Values.All(v => v != TileType.QuestBoard1)) {
            save.TilesData.Tiles.Add(new Vector2Int(4,2), TileType.QuestBoard1);
        }*/
       
    }
}