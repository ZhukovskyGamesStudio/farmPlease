using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsConfig", menuName = "Scriptable Objects/LevelsConfig", order = 0)]
public class LevelsConfig : ScriptableObject {
    [field: SerializeField]
    public List<LevelConfig> LevelConfigs { get; private set; }

    [field: SerializeField]
    public List<RewardConfig> LevelRewards { get; private set; }

    [field: SerializeField]
    public List<Sprite> LevelsIcon { get; private set; }

    [field: SerializeField]
    public List<UnlockableIcon> UnlockableIcons { get; private set; }
}

[Serializable]
public class UnlockableIcon {
    [UnlockableDropdown]
    public string Unlockable;

    public Sprite Icon;
}