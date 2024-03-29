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

        public int SelectedAmount{ get; private set; }
        private int _haveAmount;

        public void SetData(Crop type, int amount){
            if (amount == 0){
                throw new ArgumentException();
            }

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
    }
}