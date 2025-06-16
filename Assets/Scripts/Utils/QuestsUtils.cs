public static class QuestsUtils {
    public static string GetQuestProgress(QuestData data) {
        return "0/0";
    }

    public static void ClaimQuest(QuestData data) {
        if (data.Reward != null) {
            RewardUtils.ClaimReward(data.Reward);
        }
    }
}
