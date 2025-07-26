using Managers;

public static class LevelsUtils {
    public static void TryUnlockAfterLevel() {
        if (IsDailyUnlocked) {
            QuestsManager.Instance.GenerateSideQuests();
            QuestsManager.Instance.TryStartQuestsTimer();
        }

        if (IsIntersUnlocked) {
            ZhukovskyAdsManager.Instance.EnableIntersAndBanners();
        }

        if (CurrentLevel == CostsConfig.LevelToRateUs - 1) {
            RateUsManager.Instance.TryShowDialog();
        }
    }

    public static bool IsDailyUnlocked => CurrentLevel >= CostsConfig.LevelToUnlockDaily - 1;

    public static bool IsIntersUnlocked => CurrentLevel >= CostsConfig.LevelToStartInters - 1;

    private static CostsConfig CostsConfig => ConfigsManager.Instance.CostsConfig;
    private static int CurrentLevel => SaveLoadManager.CurrentSave.CurrentLevel;
}