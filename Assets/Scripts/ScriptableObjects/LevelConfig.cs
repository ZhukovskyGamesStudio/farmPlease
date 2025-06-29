using System;
using System.Collections.Generic;
using Tables;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfig", order = 0)]
public class LevelConfig : ScriptableObject {
    [LocalizationKey("Main")]
    public string LevelNameLoc;
    public string LevelName;
    public Sprite LevelMiniIcon;
}

[Serializable]
public class Reward {
    public List<RewardItem> Items = new();
}

[Serializable]
public class RewardItem {
    [RewardDropdown(typeof(CommonReward), typeof(Crop), typeof(ToolBuff))]
    public string Type;

    public int Amount;
}

[Serializable]
public enum CommonReward {
    Coins = 0,
}

[Serializable]
public class RewardWithUnlockable : Reward {
    [UnlockableDropdown]
    public string Unlockable;
}