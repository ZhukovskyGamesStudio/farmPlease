﻿public static class XpUtils {
    public static int GetNextLevelByXp(int xp) {
        return XpByLevel(LevelByXp(xp) + 1);
    }

    public static bool IsNextLevel(int currentLevel, int currentXp) {
        int levelByXp = LevelByXp(currentXp);
        return levelByXp > currentLevel;
    }

    public static int LevelByXp(int xp) {
        var levels = ConfigsManager.Instance.LevelConfigs;
        var costs = ConfigsManager.Instance.CostsConfig.LevelXpProgression;
        int levelByXp = 0;
        int accubulatedXp = 0;
        for (int index = 0; index < levels.Count; index++) {
            accubulatedXp += costs[index];
            if (accubulatedXp > xp) {
                return levelByXp;
            }

            levelByXp++;
        }

        return levelByXp;
    }

    public static int XpByLevel(int level) {
        var levels = ConfigsManager.Instance.LevelConfigs;
        var costs = ConfigsManager.Instance.CostsConfig.LevelXpProgression;
        int accubulatedXp = 0;
        for (int i = 0; i < level; i++) {
            if (i >= levels.Count) {
                return accubulatedXp;
            }

            accubulatedXp += costs[i];
        }

        return accubulatedXp;
    }
}