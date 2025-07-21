using Abstract;

public class YgManager : PreloadableSingleton<YgManager> {
    private IYGProvider _provider;

    public static IYGProvider Provider => Instance._provider;

    protected override void OnFirstInit() {
        base.OnFirstInit();
#if YG_PLATFORM
        _provider = new YGProvider();
#else
        _provider = new YGProviderMock();
#endif
    }
}