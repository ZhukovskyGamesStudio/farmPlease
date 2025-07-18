using System.Collections;
using System.Linq;
using Abstract;
using MadPixel;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZhukovskyGamesPlugin;

namespace Managers {
    public class LoadingManager : MonoBehaviour {
        private string _sceneName;
        [field:Resettable]
        public static bool IsGameLoaded { get; private set; }

        [SerializeField]
        private float _delayBeforeSceneSwitch = 2.5f;

        public void StartLoading() {
            if (IsGameLoaded)
                return;
            StartCoroutine(LoadManagers());
        }

        private IEnumerator LoadManagers() {
            CustomMonoBehaviour[] preloadedManagers =
                FindObjectsByType<CustomMonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OrderBy(m=>m.InitPriority).ToArray();
            
            foreach (CustomMonoBehaviour manager in preloadedManagers) {
                if (manager is IPreloadable preloadable) {
                    preloadable.Init();
                }
               
            }
            yield return new WaitForSeconds(_delayBeforeSceneSwitch);
            yield return new WaitUntil(() => AdsManager.Ready());
            IsGameLoaded = true;
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