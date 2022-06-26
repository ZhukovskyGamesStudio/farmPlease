using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour {
    public PreloadedManager[] PreloadedManagers;
    private string _sceneName;
    private static bool isGameLoaded;

    private void Start() {
        if(isGameLoaded)
            return;
        StartCoroutine(LoadManagers());
    }

    private IEnumerator LoadManagers() {
        foreach (PreloadedManager manager in PreloadedManagers) {
            yield return manager.Init();
        }

        isGameLoaded = true;
        if (SceneManager.GetActiveScene().name == "LoadingScene") {
            LoadGameScene();
        }
    }

    private void LoadGameScene() {
        _sceneName = SaveLoadManager.IsNewPlayer() ? "Training" : "Game";
        SceneManager.LoadSceneAsync(_sceneName);
        SceneManager.sceneLoaded += ActivateScene;
    }

    private void ActivateScene(Scene arg0, LoadSceneMode arg1) {
        SceneManager.sceneLoaded -= ActivateScene;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
    }
}