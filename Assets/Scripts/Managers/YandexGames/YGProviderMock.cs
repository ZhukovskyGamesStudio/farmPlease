#if YG_Platform
using System;

public class YGProviderMock : IYGProvider {
    public void ShowAdvReward(Action onShown) {
        onShown?.Invoke();
    }
}
#endif