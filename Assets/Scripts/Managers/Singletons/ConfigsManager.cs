using Abstract;
using ScriptableObjects;
using UnityEngine;

public class ConfigsManager : PreloadableSingleton<ConfigsManager> {
    [field: SerializeField]
    public CostsConfig CostsConfig { get; private set; }

    [field: SerializeField]
    public FtueConfig FtueConfig { get; private set; }

    [field: SerializeField]
    public LevelsConfig LevelsConfig { get; private set; }

    public override int InitPriority => -10000;
}
