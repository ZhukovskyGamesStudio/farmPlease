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

        public void ShowSpeak(string text, Action onHideEnded = null) {
            gameObject.SetActive(true);
            _speakText.text = text;
            OnAnimationEnded = onHideEnded;
            _animation.Play(SHOW);
        }

        public void HideSpeak() {
            _animation.Play(HIDE);
            StartCoroutine(WaitForAnimationEnded());
        }
    }
}