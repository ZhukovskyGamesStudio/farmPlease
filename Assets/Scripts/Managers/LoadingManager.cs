using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abstract;
using Cysharp.Threading.Tasks;
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

        [SerializeField]
        private Animation _loadingEndAnimation;
        
        [SerializeField]
        private AnimationClip _loadingEndClip;

        public void StartLoading() {
            Application.targetFrameRate = -1;
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

        private async void LoadGameScene() {
            _sceneName = "GameScene";
            var op = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
            op.allowSceneActivation = false;

            // ждём загрузку до 90% (Unity не даёт больше, пока allowSceneActivation = false)
            await UniTask.WaitUntil(() => op.progress >= 0.9f);

            // играем анимацию окончания загрузки
            _loadingEndAnimation.Play(_loadingEndClip.name);
            await UniTask.WaitWhile(() => _loadingEndAnimation.isPlaying);

            // разрешаем активацию
            op.allowSceneActivation = true;

            await UniTask.WaitUntil(() => op.isDone);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
            SceneManager.UnloadSceneAsync("LoadingScene");
        }
    }
}