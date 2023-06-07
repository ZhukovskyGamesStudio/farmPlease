using DefaultNamespace.Abstract;

namespace DefaultNamespace.Managers {
    public class DontDestroyOnLoad : Singleton<DontDestroyOnLoad> {
        protected override void OnFirstInit() {
            DontDestroyOnLoad(gameObject);
        }
    }
}