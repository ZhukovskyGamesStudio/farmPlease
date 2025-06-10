using System;
using Abstract;
using TMPro;
using UnityEngine;

namespace UI {
    public class KnowledgeCanSpeak : HasAnimationAndCallback {
        [SerializeField]
        protected TextMeshProUGUI _speakText;

        [SerializeField]
        private AnimationClip _showAnimationClip, _hideAnimationClip, _changeAnimationClip;

        private bool _isHidingAfter;
        private string _hintTextToUpdate;

        public void ShowSpeak(string text, Action onHideEnded = null, bool isHidingAfter = false) {
            gameObject.SetActive(true);
            _speakText.text = text;
            OnAnimationEnded = onHideEnded;
            _animation.Play(_showAnimationClip.name);
            _isHidingAfter = isHidingAfter;
        }

        public void ChangeSpeak(string text, Action onHideEnded = null, bool isHidingAfter = false) {
            gameObject.SetActive(true);
            _hintTextToUpdate = text;

            OnAnimationEnded = onHideEnded;
            _animation.Play(_changeAnimationClip.name);
            _isHidingAfter = isHidingAfter;
        }

        public void UpdateText() {
            _speakText.text = _hintTextToUpdate;
        }

        public void HideSpeak() {
            if (_isHidingAfter) {
                _animation.Play(_hideAnimationClip.name);
                StartCoroutine(WaitForAnimationEnded());
            } else {
                OnAnimationEnded?.Invoke();
            }
        }

        public void HideSpeakWithoutCallback() {
            OnAnimationEnded = null;
            _animation.Play(_hideAnimationClip.name);
        }
    }
}