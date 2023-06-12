using Abstract;
using UnityEngine;

namespace Managers {
    public class LoadingSceneEntryPoint : SceneEntryPoint {
        [SerializeField]
        private LoadingManager _loadingManager;

        protected override void Start() {
            _loadingManager.StartLoading();
        }
    }
}