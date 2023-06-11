using System.Collections;
using DefaultNamespace.Abstract;
using DefaultNamespace.Tables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace.UI {
    public class FirstSessionManager : Singleton<FirstSessionManager>, IPreloadable {
        [SerializeField]
        private FtueConfig _ftueConfig;

        public bool IsFirstSession => SaveLoadManager.IsNoSaveExist();

        private int _curFtueStep;
        private bool _isWatingForStepEnd;

        private void TryStartFtue() {
            if (IsFirstSession) {
                StartCoroutine(FtueCoroutine());
            }
        }

        private IEnumerator FtueCoroutine() {
            WaitForLoadingEnd();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            ShowStartTextSpotlight();
            yield return new WaitWhile(() => _isWatingForStepEnd);
            ShowNextFtueStep();
        }

        private void WaitForLoadingEnd() {
            _isWatingForStepEnd = true;
            SceneManager.sceneLoaded += delegate { StepEnded(); };
        }

        private void StepEnded() {
            _isWatingForStepEnd = false;
        }

        public void ShowNextFtueStep() {
            ShowCollectSpotlight();
        }

        private void ShowStartTextSpotlight() {
            _isWatingForStepEnd = true;
            UIHud.Instance.KnowledgeCanSpeak.ShowSpeak(_ftueConfig.StartHint, StepEnded);
        }

        private void ShowCollectSpotlight() {
            UIHud.Instance.SpotlightWithText.ShowSpotlight(UIHud.Instance.FastPanelScript.HoeImage.transform, _ftueConfig.ScytheHint);
        }

        public void Init() {
            TryStartFtue();
        }
    }
}