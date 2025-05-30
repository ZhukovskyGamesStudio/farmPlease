using Managers;
using UnityEngine.SceneManagement;

public static class ChangeToLoading {
    public static bool TryChangeScene() {
        if (!LoadingManager.IsGameLoaded) {
            SceneManager.LoadScene("LoadingScene");
            return true;
        }

        return false;
    }
}