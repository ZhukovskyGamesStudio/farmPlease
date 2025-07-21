using System;

public class InAppsProviderMock : IInAppsProvider {
    public void Init() { }
    public string GetPrice(string name) {
       return "0$";
    }
    public void Buy(string name, Action onSuccess) {
        onSuccess?.Invoke();
    }
}
