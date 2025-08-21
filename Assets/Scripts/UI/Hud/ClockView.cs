using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ClockView : MonoBehaviour {
        [SerializeField]
        private List<GameObject> _greenPieces;

        [SerializeField]
        private Animation _animation;

        [SerializeField]
        private Transform _clockArrow;

        [SerializeField]
        private Button _button;

        private const string ZERO_TIME_ANIMATION = "ZeroTime";
        private const string INITIAL = "Initial";
        private const string WASTE_ONE = "WasteOne";

        [SerializeField]
        private AnimationClip _refillClock;

        [SerializeField]
        private AnimationClip _timeStillLeftClip;

        [SerializeField]
        private GameObject _goldenClock;

        [SerializeField]
        private GameObject _adIcon;

        [SerializeField]
        private GoldenTimer _goldenTimer;

        public bool IsLockedByFtue { get; set; }

        private void Update() {
            UpdateGoldenState();
            UpdateGoldenTimer();
        }

        private void UpdateGoldenTimer() {
            _goldenTimer.gameObject.SetActive(!RealShopUtils.IsAllEndless &&
                                              RealShopUtils.IsGoldenClockActive(SaveLoadManager.CurrentSave.RealShopData));
            _goldenTimer.SetTime(RealShopUtils.ClockTimeLeft(SaveLoadManager.CurrentSave.RealShopData));
        }

        public void UpdateGoldenState() {
            _goldenClock.SetActive(RealShopUtils.IsGoldenClockActive(SaveLoadManager.CurrentSave.RealShopData));
        }

        public void UpdateAdIcon() {
            _adIcon.SetActive(Clock.Instance.IsAdIconActive());
        }

        public void ClockPressedButton() {
            if (IsLockedByFtue) {
                return;
            }

            Clock.Instance.TryAddDay();
            UpdateAdIcon();
        }

        public void SetClockArrowRotation(float rotation) {
            _clockArrow.localRotation = Quaternion.Euler(0, 0, rotation);
        }

        public void SetAmount(int amount) {
            ShowInitialAnimation();
            SetPiecesAmount(amount);
            UpdateAdIcon();
        }

        public void SetAmountWithWasteAnimation(int amount) {
            SetPiecesAmount(amount);
            ShowWasteAnimation();
        }

        public void SetFullAmount(int amount) {
            SetPiecesAmount(amount);
            ShowRefillAnimation();
        }

        private void SetPiecesAmount(int amount) {
            for (int i = 0; i < _greenPieces.Count; i++) {
                _greenPieces[_greenPieces.Count - 1 - i].SetActive(amount > i);
            }
        }

        public void ShowZeroTimeAnimation() {
            _animation.Play(ZERO_TIME_ANIMATION);
        }

        public void TimeStillLeftAnimation() {
            _animation.Play(_timeStillLeftClip.name);
        }

        private void ShowInitialAnimation() {
            _animation.Play(INITIAL);
        }

        private void ShowWasteAnimation() {
            _animation.Play(WASTE_ONE);
        }

        private void ShowRefillAnimation() {
            _animation.Play(_refillClock.name);
        }

        public void SetInteractable(bool isInteractable) {
            _button.interactable = isInteractable;
        }
    }
}