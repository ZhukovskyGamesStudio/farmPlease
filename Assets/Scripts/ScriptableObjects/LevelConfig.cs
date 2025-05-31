using System;
using System.Collections.Generic;
using Tables;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfig", order = 0)]
public class LevelConfig : ScriptableObject {
    public int XpNeeded;
    public string LevelName;

    public RewardWithUnlockable Reward;
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