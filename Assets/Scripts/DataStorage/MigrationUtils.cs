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
        var pos = QuestsUtils.QuestBoardPosition;

        if (save.TilesData.Tiles.ContainsKey(pos)) {
            QuestsUtils.PlaceQuestBoard();
        } else {
            save.TilesData.Tiles.Add(pos, TileType.QuestBoard1_new);
        }

        /*
        if (save.TilesData.Tiles.Values.All(v => v != TileType.QuestBoard1)) {
            save.TilesData.Tiles.Add(new Vector2Int(4,2), TileType.QuestBoard1);
        }*/
    }
}