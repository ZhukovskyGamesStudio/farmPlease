using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class FoodMarketOffer : MonoBehaviour {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private TextMeshProUGUI _nameText;

        [SerializeField]
        private Button _button;

        private Action _onButtonClick;

        public void Init(Sprite icon, string nameText, Action onButtonClick) {
            _image.sprite = icon;
            _nameText.text = nameText;
            _onButtonClick = onButtonClick;
        }

        public void OfferButtonClicked() {
            _onButtonClick?.Invoke();
        }

        public void UpdateInteractable(bool isInteractable) {
            _button.interactable = isInteractable;
        }
    }
