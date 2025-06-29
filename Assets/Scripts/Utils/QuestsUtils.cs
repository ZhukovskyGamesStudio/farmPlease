using System;
using Managers;
using Tables;
using UnityEngine;

public static class QuestsUtils {
    public static Vector2Int QuestBoardPosition => new Vector2Int(-1, 4);

    public static string GetQuestProgress(QuestData data) {
        return $"{data.Progress}/{data.ProgressNeeded}";
    }

    public static float GetQuestProgressPercent(QuestData data) {
        return (data.Progress + 0f) / data.ProgressNeeded;
    }

    public static float GetQuestReadyForSend(QuestData data) {
        if (Enum.TryParse(data.TargetType, out Crop crop)) {
            return InventoryManager.Instance.CountCrops(crop);
        }

        return 0;
    }

    public static bool IsReadyForSend(QuestData data) {
        if (Enum.TryParse(data.TargetType, out Crop crop)) {
            return InventoryManager.Instance.CountCrops(crop) >= data.ProgressNeeded;
        }

        return false;
    }
    
    public static void SendQuest(QuestData data) {
        if (Enum.TryParse(data.TargetType, out Crop crop)) {
            for (int i = 0; i < data.ProgressNeeded; i++) {
                SaveLoadManager.CurrentSave.CropsCollected.Remove(crop);
            }
        }

        data.IsCompleted = true;
        SaveLoadManager.SaveGame();
    }


    public static void ClaimQuest(QuestData data) {
        if (data.Reward != null) {
            RewardUtils.ClaimReward(data.Reward);
        }

        if (data.XpReward > 0) {
            InventoryManager.Instance.AddXp(data.XpReward);
        }

        if (data.IsMain) {
            QuestsManager.Instance.ProgressMainQuestline();
        }
    }

    
    public static void PlaceQuestBoard() {
        SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_11);
        SmartTilemap.Instance.PlaceTile(QuestBoardPosition + Vector2Int.right, TileType.QuestBoard2);
        SmartTilemap.Instance.PlaceTile(QuestBoardPosition + Vector2Int.right * 2, TileType.QuestBoard3);
        SmartTilemap.Instance.PlaceTile(QuestBoardPosition + Vector2Int.right * 3, TileType.QuestBoard4);
        //SmartTilemap.Instance.PlaceBuilding(BuildingType.QuestBoard, QuestBoardPosition);
    }

    public static void AddQustBoardToSave() {
        SmartTilemap.AddTileToSave(QuestBoardPosition, TileType.QuestBoard1_11);
        SmartTilemap.AddTileToSave(QuestBoardPosition + Vector2Int.right, TileType.QuestBoard2);
        SmartTilemap.AddTileToSave(QuestBoardPosition + Vector2Int.right * 2, TileType.QuestBoard3);
        SmartTilemap.AddTileToSave(QuestBoardPosition + Vector2Int.right * 3, TileType.QuestBoard4);
    }

    public static void ChangeTileView(QuestsDialogData data) {
        if (data.IsUnseenUpdate) {
            SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_new);
        } else if (data.FirstQuest != null && data.SecondQuest != null) {
            SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_11);
        } else if (data.FirstQuest != null && data.SecondQuest == null) {
            SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_10);
        } else if (data.FirstQuest != null && data.SecondQuest == null) {
            SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_01);
        } else {
            SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_00);
        }
    }
}