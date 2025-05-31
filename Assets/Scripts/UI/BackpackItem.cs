using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class BackpackItem : MonoBehaviour {
        [SerializeField]
        private Image _icon;
        
        [SerializeField]
        private TextMeshProUGUI amountText;

        [SerializeField]
        private Button button;
        
        [SerializeField]
        private BackpackItemBackView _backView;

        private Func<int> _checkAmount;

        public void InitButton(int amount, Sprite sprite, Action onButtonClick, Func<int> onCheckAmount, ItemColorType colorType) {
            amountText.text = amount.ToString();
            _checkAmount = onCheckAmount;
            _icon.sprite = sprite;
            _backView.InitColor(colorType);
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
[Serializable]
public enum ItemColorType {
    Seed = 0,
    Tool,
    Energy,
    None = -1
}
