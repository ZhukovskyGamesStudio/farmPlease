#if YG_Platform
using System;

public interface IYGProvider {
    public void ShowAdvReward(Action onShown);
}
#endif