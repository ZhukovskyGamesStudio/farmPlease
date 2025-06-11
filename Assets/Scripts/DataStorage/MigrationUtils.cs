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
}