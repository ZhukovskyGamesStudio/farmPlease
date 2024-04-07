using System;
using Tables;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class BackpackItem : MonoBehaviour {
        private Crop crop;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TextMeshProUGUI amountText;

        [SerializeField]
        private Button button;

        private Func<int> _checkAmount;

        public void InitButton(int amount, Sprite sprite, Action onButtonClick, Func<int> onCheckAmount) {
            amountText.text = amount.ToString();
            _checkAmount = onCheckAmount;
            _icon.sprite = sprite;
            button.onClick.AddListener(() => onButtonClick?.Invoke());
        }

        public void UpdateAmount(int newAmount) {
            amountText.text = newAmount.ToString();
            gameObject.SetActive(newAmount != 0);
        }

        public int SyncAmount() {
            int newAmount = _checkAmount.Invoke();
            UpdateAmount(newAmount);
            return newAmount;
        }
    }
}