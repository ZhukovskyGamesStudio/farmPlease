using System.Collections.Generic;
using System.Linq;
using Managers;
using Tables;
using UnityEngine;

public static class ToolsUtils {
    public static void ChangeTools() {
        ToolShopData toolShopData = SaveLoadManager.CurrentSave.ToolShopData;
        List<ToolBuff> possibleTools = ToolsTable.Tools.Where(CheckToolAvailable).ToList();

        if (possibleTools.Count == 1) {
            toolShopData.FirstOffer = possibleTools[0];
            toolShopData.SecondOffer = possibleTools[0];
        } else {
            toolShopData.FirstOffer = possibleTools[Random.Range(0, possibleTools.Count)];
            possibleTools.Remove(toolShopData.FirstOffer);
            toolShopData.SecondOffer = possibleTools[Random.Range(0, possibleTools.Count)];
        }

        toolShopData.FirstOfferActive = true;
        toolShopData.SecondOfferActive = true;
        toolShopData.ChangeButtonActive = true;
        SaveLoadManager.SaveGame();
    }

    private static bool CheckToolAvailable(ToolBuff key) {
        return key != ToolBuff.WeekBattery && UnlockableUtils.HasUnlockable(key) &&
               (ToolsTable.ToolByType(key).isAlwaysAvailable || InventoryManager.IsToolsBoughtD[key]);
    }
}