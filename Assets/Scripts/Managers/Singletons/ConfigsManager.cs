using Abstract;
using UnityEngine;

public class ConfigsManager : PreloadableSingleton<ConfigsManager> {
    [SerializeField]
    private CostsConfig _costsConfig;

    public CostsConfig CostsConfig => _costsConfig;
}