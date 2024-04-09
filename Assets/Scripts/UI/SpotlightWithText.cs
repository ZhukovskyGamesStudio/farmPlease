using System;
using Abstract;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class SpotlightWithText : HasAnimationAndCallback {
        [SerializeField]
        protected TextMeshProUGUI _hintText;

        [SerializeField]
        private CanvasGroup _centerImageCanvasGroup;

        [SerializeField]
        protected RectTransform _shadowCenter, _headShift;

        private const string SHOW = "SpotlightShow";
        private const string HIDE = "SpotlightHide";

        private Button _target;
        private Action _onButtonPressed;
        private bool _isHidingByAnyTap;

        public void ShowSpotlightOnButton(Button target, SpotlightAnimConfig animDataConfig, Action onButtonPressed = null) {
            gameObject.SetActive(true);
            ShowSpotlight(target.transform.position, animDataConfig);
            _target = target;
            target.onClick.AddListener(OnTargetButtonPressed);
            OnAnimationEnded = onButtonPressed;
            _isHidingByAnyTap = false;
            ChangeCenterBlockRaycast(_isHidingByAnyTap);
        }

        public void ShowSpotlight(Transform target, SpotlightAnimConfig animDataConfig, Action onHideEnded = null,
            bool isHidingByAnyTap = true) {
            gameObject.SetActive(true);
            ShowSpotlight(target.position, animDataConfig);
            OnAnimationEnded = onHideEnded;
            _isHidingByAnyTap = isHidingByAnyTap;
            ChangeCenterBlockRaycast(_isHidingByAnyTap);
        }

        private void OnTargetButtonPressed() {
            HideSpotlight();
            _target.onClick.RemoveListener(OnTargetButtonPressed);
            _target = null;
        }

        private void ShowSpotlight(Vector3 targetPos, SpotlightAnimConfig config) {
            transform.position = targetPos;
            _headShift.anchoredPosition = config.HeadShift;
            _shadowCenter.sizeDelta = config.SpotlightSize;
            _hintText.text = config.HintText;
            _animation.Play(SHOW);
        }

        private void ChangeCenterBlockRaycast(bool isBlock) {
            _centerImageCanvasGroup.blocksRaycasts = isBlock;
        }

        public void Hide() {
            HideSpotlight();
        }

        public void HideButton() {
            if (_isHidingByAnyTap) {
                HideSpotlight();
            }
        }

        public void HideSpotlight() {
            _animation.Play(HIDE);
            StartCoroutine(WaitForAnimationEnded());
        }
    }
}