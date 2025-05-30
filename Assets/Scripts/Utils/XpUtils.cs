using Managers;

public static class XpUtils {
    public static int GetNextLevelByXp(int xp) {
        return XpByLevel(LevelByXp(xp)+1);
    }

    public static bool IsNextLevel(int currentLevel, int currentXp) {
        var levels = ConfigsManager.Instance.LevelConfigs;
        int levelByXp = LevelByXp(currentXp);
        return levelByXp > currentLevel;
    }

    public static int LevelByXp(int xp) {
        var levels = ConfigsManager.Instance.LevelConfigs;
        int levelByXp = 0;
        int accubulatedXp = 0;
        foreach (LevelConfig t in levels) {
            accubulatedXp += t.XpNeeded;
            if (accubulatedXp > xp) {
                return levelByXp;
            }

            levelByXp++;
        }

        return levelByXp;
    }
    
    public static int XpByLevel(int level) {
        var levels = ConfigsManager.Instance.LevelConfigs;
        int accubulatedXp = 0;
        for (int i = 0; i < level; i++) {
            if (i >= levels.Count) {
                return accubulatedXp;
            }
            accubulatedXp += levels[i].XpNeeded;
        }

        return accubulatedXp;
    }
}