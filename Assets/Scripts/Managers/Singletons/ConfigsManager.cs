using Abstract;
using Cysharp.Threading.Tasks;
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

    public async UniTask LoadConfigsAsync() {
        CostsConfig = await Resources.LoadAsync<CostsConfig>("Configs/CostsConfig") as CostsConfig;
        FtueConfig = await Resources.LoadAsync<FtueConfig>("Configs/FtueConfig") as FtueConfig;
        LevelsConfig = await Resources.LoadAsync<LevelsConfig>("Configs/LevelsConfig") as LevelsConfig;
        await UniTask.Yield();
    }
}