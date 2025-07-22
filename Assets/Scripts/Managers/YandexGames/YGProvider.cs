using System;
using YG;

public class YGProvider : IYGProvider {
    public void ShowAdvReward(Action onShown) {
        string id = Guid.NewGuid().ToString();
        YG2.RewardedAdvShow(id, onShown);
    }
}