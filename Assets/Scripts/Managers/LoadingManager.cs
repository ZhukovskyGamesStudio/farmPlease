using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZhukovskyGamesPlugin;

namespace Managers {
    public class LoadingManager : MonoBehaviour {
        private string _sceneName;

        [field: Resettable]
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
                FindObjectsByType<CustomMonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OrderBy(m => m.InitPriority)
                    .ToArray();

            foreach (CustomMonoBehaviour manager in preloadedManagers) {
                if (manager is IPreloadable preloadable) {
                    preloadable.Init();
                }
            }
            
            yield return new WaitForSeconds(_delayBeforeSceneSwitch);
            yield return new WaitUntil(() => ZhukovskyAdsManager.Instance.AdsProvider.IsAdsReady());
            ZhukovskyAnalyticsManager.Instance.SendCustomEvent("technical", new Dictionary<string, object> {
                {"step_name", "01_gameLaunch"},
                {"first_start", SaveLoadManager.CurrentSave.FirstLaunch}
            }, true);
            SaveLoadManager.CurrentSave.FirstLaunch = false;
            SaveLoadManager.SaveGame();
            IsGameLoaded = true;
            if (SceneManager.GetActiveScene().name == "LoadingScene") {
                LoadGameScene();
            }
        }

        private void LoadGameScene() {
            _sceneName = "GameScene";
            SceneManager.LoadSceneAsync("GameScene");
            SceneManager.sceneLoaded += ActivateScene;
        }

        private void ActivateScene(Scene arg0, LoadSceneMode arg1) {
            SceneManager.sceneLoaded -= ActivateScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
        }
    }
}