using System.Collections.Generic;
using System.Linq;
using Managers;
using Tables;
using UnityEngine;

public static class SeedsUtils {
    public static void ChangeSeeds() {
        SeedShopData toolShopData = SaveLoadManager.CurrentSave.SeedShopData;
        List<Crop> possibleCrops = CropsTable.CropsTypes.Where(CheckCropAvailable).ToList();

        if (possibleCrops.Count == 1) {
            toolShopData.FirstOffer = possibleCrops[0];
            toolShopData.SecondOffer = possibleCrops[0];
        } else {
            toolShopData.FirstOffer = possibleCrops[Random.Range(0, possibleCrops.Count)];
            possibleCrops.Remove(toolShopData.FirstOffer);
            toolShopData.SecondOffer = possibleCrops[Random.Range(0, possibleCrops.Count)];
        }

        toolShopData.ChangeButtonActive = true;
        SaveLoadManager.SaveGame();
    }

    private static bool CheckCropAvailable(Crop key) {
        return key != Crop.Weed && UnlockableUtils.HasUnlockable(key) &&
               (CropsTable.CropByType(key).CanBeBought || InventoryManager.IsCropsBoughtD[key]);
    }
}