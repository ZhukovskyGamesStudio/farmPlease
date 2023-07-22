using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ClockView : MonoBehaviour {
        [SerializeField] private List<GameObject> _greenPieces;

        [SerializeField] private Animation _animation;
        [SerializeField] private Button _button;
        private const string ZERO_TIME_ANIMATION = "ZeroTime";
        private const string INITIAL = "Initial";
        private const string WASTE_ONE = "WasteOne";

        [SerializeField] private HelloPanelView _helloPanel;

        public bool IsLockedByFtue { get; set; }

        public void ClockPressedButton() {
            if (IsLockedByFtue) {
                return;
            }

            Clock.Instance.TryAddDay();
        }

        public void SetAmount(int amount) {
            ShowInitialAnimation();
            SetPiecesAmount(amount);
        }

        public void SetAmountWithWasteAnimation(int amount) {
            SetPiecesAmount(amount);
            ShowWasteAnimation();
        }

        private void SetPiecesAmount(int amount) {
            for (int i = 0; i < _greenPieces.Count; i++) {
                _greenPieces[_greenPieces.Count - 1 - i].SetActive(amount > i);
            }
        }

        public void ShowZeroTimeAnimation() {
            _animation.Play(ZERO_TIME_ANIMATION);
        }

        private void ShowInitialAnimation() {
            _animation.Play(INITIAL);
        }

        private void ShowWasteAnimation() {
            _animation.Play(WASTE_ONE);
        }

        public void SetInteractable(bool isInteractable) {
            _button.interactable = isInteractable;
        }

        public void ShowHelloPanel() {
            _helloPanel.Show("");
        }
    }
}