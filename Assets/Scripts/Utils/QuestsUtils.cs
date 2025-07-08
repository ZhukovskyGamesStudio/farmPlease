using System;
using Managers;
using Tables;
using UnityEngine;

public static class QuestsUtils {
    public static Vector2Int QuestBoardPosition => new Vector2Int(-1, 4);

    public static string GetQuestProgress(QuestData data) {
        return $"{data.Progress}/{data.ProgressNeeded}";
    }
    public static string GetQuestSendProgress(QuestData data) {
        return $"{GetQuestReadyForSend(data)}/{data.ProgressNeeded}";
    }

    public static float GetQuestProgressPercent(QuestData data) {
        return (data.Progress + 0f) / data.ProgressNeeded;
    }

    public static int GetQuestReadyForSend(QuestData data) {
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
            InventoryManager.Instance.AddCropPoint(-data.ProgressNeeded);
        }

        data.IsCompleted = true;
        ChangeTileView(SaveLoadManager.CurrentSave.QuestsData);
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
        ChangeTileView(SaveLoadManager.CurrentSave.QuestsData);
        
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
        } else if (IsActiveQuest(data.FirstQuest) && IsActiveQuest(data.SecondQuest)) {
            SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_11);
        } else if (IsActiveQuest(data.FirstQuest) && !IsActiveQuest(data.SecondQuest)) {
            SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_10);
        } else if (!IsActiveQuest(data.FirstQuest) && IsActiveQuest(data.SecondQuest)) {
            SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_01);
        } else {
            SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_00);
        }
    }

    private static bool IsActiveQuest(QuestData data) {
        if (data == null) {
            return false;
        }

        return !data.IsClaimed;
    }
}