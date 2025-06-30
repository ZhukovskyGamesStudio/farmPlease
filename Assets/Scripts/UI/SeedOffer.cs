using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Localization;
using ScriptableObjects;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class SeedOffer : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI _costText;

        [SerializeField]
        private TextMeshProUGUI _explainText;

        public GameObject RareEdge;
        public GameObject LegendaryEdge;
        public CanvasGroup CanvasGroup;
        public Image OfferImage;
        private Func<bool> _buyCallback;
        private Action _startDragCallback;

        [SerializeField]
        private GameObject _infoButton;

        [SerializeField]
        private GameObject _infoHint;

        [SerializeField]
        private Animation _offerAnimation;

        [SerializeField]
        private AnimationClip _boughtByClickAnimationClip;

        public Crop CurrentCrop { get; private set; }
        private Transform _bag;
        
        private bool _isDragging;

        public void SetData(CropConfig cropConfig, Func<bool> buyCallback, Action startDragCallback, Transform bag) {
            _bag = bag;
            CurrentCrop = cropConfig.type;
            _costText.text = cropConfig.cost.ToString();
            _explainText.text = LocalizationUtils.L(cropConfig.explainTextLoc);
            OfferImage.sprite = cropConfig.SeedSprite;
            _buyCallback = buyCallback;
            _startDragCallback = startDragCallback;
            SetRarity(cropConfig.Rarity);
        }

        public void StartDrag() {
            _isDragging = true;
            _startDragCallback?.Invoke();
            _infoButton.gameObject.SetActive(false);
        }

        public void EndDrag() {
            DelayedStopDragging();
            _infoButton.gameObject.SetActive(true);
        }

        private async UniTask DelayedStopDragging() {
            await UniTask.WaitForEndOfFrame();
            _isDragging = false;
        }

        public void TryBuy() {
            _buyCallback?.Invoke();
        }

        public void TryBuyByClick() {
            if (_buyCallback == null || _isDragging) {
                return;
            }

            if (_buyCallback.Invoke()) {
                _offerAnimation.Stop();
                _offerAnimation.Play(_boughtByClickAnimationClip.name);
            }
        }

        public void ShowHint() {
            _infoButton.gameObject.SetActive(false);
            _infoHint.gameObject.SetActive(true);
        }

        public void CloseHint() {
            _infoButton.gameObject.SetActive(true);
            _infoHint.gameObject.SetActive(false);
        }

        private void SetRarity(int rarity) {
            switch (rarity) {
                case 1:
                    RareEdge.SetActive(true);
                    break;
                case 2:
                    LegendaryEdge.SetActive(true);
                    break;
            }
        }
    }
}