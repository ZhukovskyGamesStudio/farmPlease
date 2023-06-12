using ZhukovskyGamesPlugin;

namespace Managers {
    public class DontDestroyOnLoad : Singleton<DontDestroyOnLoad> {
        protected override void OnFirstInit() {
            DontDestroyOnLoad(gameObject);
        }
    }
}