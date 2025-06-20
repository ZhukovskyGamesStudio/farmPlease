using Tables;
using UnityEngine;

public static class QuestsUtils {
    public static Vector2Int QuestBoardPosition => new Vector2Int(-1, 4);

    public static string GetQuestProgress(QuestData data) {
        return "0/0";
    }

    public static void ClaimQuest(QuestData data) {
        if (data.Reward != null) {
            RewardUtils.ClaimReward(data.Reward);
        }
    }

    public static void PlaceQuestBoard() {
        SmartTilemap.Instance.PlaceTile(QuestBoardPosition, TileType.QuestBoard1_11);
        SmartTilemap.Instance.PlaceTile(QuestBoardPosition + Vector2Int.right, TileType.QuestBoard2);
        SmartTilemap.Instance.PlaceTile(QuestBoardPosition + Vector2Int.right * 2, TileType.QuestBoard3);
        //SmartTilemap.Instance.PlaceBuilding(BuildingType.QuestBoard, QuestBoardPosition);
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