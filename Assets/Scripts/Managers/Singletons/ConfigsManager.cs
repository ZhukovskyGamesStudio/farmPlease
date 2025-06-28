using System;
using System.Collections.Generic;
using Abstract;
using ScriptableObjects;
using UnityEngine;

public class ConfigsManager : PreloadableSingleton<ConfigsManager> {
    [field: SerializeField]
    public CostsConfig CostsConfig { get; private set; }

    [field: SerializeField]
    public FtueConfig FtueConfig { get; private set; }

    [field: SerializeField]
    public List<UnclockableIcon> UnclockableIcons { get; private set; }

    [field: SerializeField]
    public List<LevelConfig> LevelConfigs { get; private set; }

    [field: SerializeField]
    public List<Sprite> LevelsIcon { get; private set; }

    public override int InitPriority => -10000;
}

[Serializable]
public class UnclockableIcon {
    [UnlockableDropdown]
    public string Unlockable;

    public Sprite Icon;
}