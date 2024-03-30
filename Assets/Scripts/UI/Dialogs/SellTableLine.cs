using System;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI{
    public class SellTableLine : MonoBehaviour{
        [SerializeField]
        private Image _cropImage;

        [SerializeField]
        private TextMeshProUGUI _cropText, _selectedText;

        [SerializeField]
        private Button _minusButton, _plusButton;

        public Action<int> OnSelectedAmountChange;

        public int SelectedAmount{ get; private set; }
        private int _haveAmount;

        public void SetData(Crop type, int amount, Action<int> onSelectedAmountChange){
            if (amount == 0){
                throw new ArgumentException();
            }
            OnSelectedAmountChange += onSelectedAmountChange;
            _haveAmount = amount;
            SelectedAmount = 0;

            _cropImage.sprite = CropsTable.CropByType(type).VegSprite;
            _cropText.text = CropsTable.CropByType(type).header.ToLower();
            UpdateButtonsAndTextsState();
        }

        private void UpdateButtonsAndTextsState(){
            _selectedText.text = $"{SelectedAmount}/{_haveAmount}";
            _minusButton.interactable = SelectedAmount > 0;
            _plusButton.interactable = SelectedAmount < _haveAmount;
            OnSelectedAmountChange?.Invoke(SelectedAmount);
        }

        public void Plus(){
            SelectedAmount++;
            UpdateButtonsAndTextsState();
        }

        public void Minus(){
            SelectedAmount--;
            UpdateButtonsAndTextsState();
        }

        public void SelectAll(){
            SelectedAmount = _haveAmount;
            UpdateButtonsAndTextsState();
        }

        private void OnDestroy() {
            OnSelectedAmountChange = null;
        }
    }
}