using System;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class SellTableLine : MonoBehaviour {
        [SerializeField]
        private Image _cropImage;

        [SerializeField]
        private TextMeshProUGUI _cropText, _selectedText;

        [SerializeField]
        private Button _minusButton, _plusButton;

        public Action<Crop, int> OnSelectedAmountChange;

        public int SelectedAmount { get; private set; }
        private Crop _crop;
        private int _haveAmount;

        public void SetData(Crop type, int amount, Action<Crop, int> onSelectedAmountChange) {
            if (amount == 0) {
                throw new ArgumentException();
            }

            OnSelectedAmountChange += onSelectedAmountChange;
            _haveAmount = amount;
            SelectedAmount = 0;

            _cropImage.sprite = CropsTable.CropByType(type).VegSprite;
            _cropText.text = CropsTable.CropByType(type).header.ToLower();
            UpdateButtonsAndTextsState();
        }

        private void UpdateButtonsAndTextsState() {
            _selectedText.text = $"{SelectedAmount}/{_haveAmount}";
            _minusButton.interactable = SelectedAmount > 0;
            _plusButton.interactable = SelectedAmount < _haveAmount;
        }

        public void Plus() {
            SelectedAmount++;
            UpdateButtonsAndTextsState();
            OnSelectedAmountChange?.Invoke(_crop, 1);
        }

        public void Minus() {
            SelectedAmount--;
            UpdateButtonsAndTextsState();
            OnSelectedAmountChange?.Invoke(_crop, -1);
        }

        public void SelectAll() {
            int diff = _haveAmount - SelectedAmount;
            SelectedAmount = _haveAmount;
            UpdateButtonsAndTextsState();
            OnSelectedAmountChange?.Invoke(_crop, diff);
        }

        private void OnDestroy() {
            OnSelectedAmountChange = null;
        }
    }
}