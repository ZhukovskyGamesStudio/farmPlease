using System;
using Abstract;
using TMPro;
using UnityEngine;

namespace UI {
    public class KnowledgeCanSpeak : HasAnimationAndCallback {
        [SerializeField]
        protected TextMeshProUGUI _speakText;

        private const string SHOW = "KnowledgeSpeakShow";
        private const string HIDE = "KnowledgeSpeakHide";
        private const string CHANGE = "KnowledgeSpeakChange";

        private bool _isHidingAfter;
        private string _hintTextToUpdate;

        public void ShowSpeak(string text, Action onHideEnded = null, bool isHidingAfter = false) {
            gameObject.SetActive(true);
            _speakText.text = text;
            OnAnimationEnded = onHideEnded;
            _animation.Play(SHOW);
            _isHidingAfter = isHidingAfter;
        }

        public void ChangeSpeak(string text, Action onHideEnded = null, bool isHidingAfter = false) {
            gameObject.SetActive(true);
            _hintTextToUpdate = text;

            OnAnimationEnded = onHideEnded;
            _animation.Play(CHANGE);
            _isHidingAfter = isHidingAfter;
        }

        public void UpdateText() {
            _speakText.text = _hintTextToUpdate;
        }

        public void HideSpeak() {
            if (_isHidingAfter) {
                _animation.Play(HIDE);
                StartCoroutine(WaitForAnimationEnded());
            } else {
                OnAnimationEnded?.Invoke();
            }
        }
    }
}