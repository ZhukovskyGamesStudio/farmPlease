using Abstract;

public class InAppsManager : PreloadableSingleton<InAppsManager> {
    public InAppsProvider InAppsProvider { get; private set; }

    protected override void OnFirstInit()
    {
        base.OnFirstInit();
        InAppsProvider = new MadPixelInAppProvider();
        InAppsProvider.Init();
    }
}
