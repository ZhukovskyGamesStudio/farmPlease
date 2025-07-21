using System;

    public interface InAppsProvider {

        public void Init();
        
        public string GetPrice(string name);
        public void Buy(string name, Action onSuccess);
    }
