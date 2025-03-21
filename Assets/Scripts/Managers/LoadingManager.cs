using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZhukovskyGamesPlugin;

namespace Managers {
    public class LoadingManager : MonoBehaviour {
        public List<CustomMonoBehaviour> PreloadedManagers;
        private string _sceneName;
        private static bool isGameLoaded;

        [SerializeField]
        private float _delayBeforeSceneSwitch = 1;

        public void StartLoading() {
            if (isGameLoaded)
                return;
            StartCoroutine(LoadManagers());
        }

        private IEnumerator LoadManagers() {
            foreach (IPreloadable manager in PreloadedManagers.Cast<IPreloadable>()) {
                manager.Init();
            }

            yield return new WaitForSeconds(_delayBeforeSceneSwitch);

            isGameLoaded = true;
            if (SceneManager.GetActiveScene().name == "LoadingScene") {
                LoadGameScene();
            }
        }

        private void LoadGameScene() {
            // _sceneName = SaveLoadManager.IsNoSaveExist() ? "Training" : "Game";
            _sceneName = "Game";
            SceneManager.LoadSceneAsync(_sceneName);
            SceneManager.sceneLoaded += ActivateScene;
        }

        private void ActivateScene(Scene arg0, LoadSceneMode arg1) {
            SceneManager.sceneLoaded -= ActivateScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
        }
    }
}