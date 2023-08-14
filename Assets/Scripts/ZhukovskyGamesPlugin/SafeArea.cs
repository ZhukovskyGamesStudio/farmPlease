using UnityEngine;

namespace ZhukovskyGamesPlugin {
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour {
        private RectTransform _myRectTransform;

        private void Awake() {
            _myRectTransform = GetComponent<RectTransform>();
            InvokeRepeating(nameof(UpdateSafeArea), 0, 5f);
        }

        private void UpdateSafeArea() {
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position = safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _myRectTransform.anchorMin = anchorMin;
            _myRectTransform.anchorMax = anchorMax;
        }
    }
}