using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimatableProgressbar : MonoBehaviour {
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private TextMeshProUGUI _amountText;

    [SerializeField]
    private Sprite _icon;

    [SerializeField]
    private CounterChangeFx _changeUpFx;

    [SerializeField]
    private bool _isSpacesAroundSlashes = false;
    
    private int _amount = 0;
    private int _maxAmount;

    public void SetAmount(int amount, int maxAmount) {
        _amount = amount;
        if (_amountText) {
            _amountText.text = _isSpacesAroundSlashes ? $"{amount} / {maxAmount}" : $"{amount}/{maxAmount}";
        }

        _slider.maxValue = maxAmount;
        _slider.value = amount;
        _maxAmount = maxAmount;
    }

    public void ChangeAmount(int changeAmount) {
        if (changeAmount == 0) {
            return;
        }

        _amount += changeAmount;
        SetAmount(_amount, _maxAmount);
        if (gameObject.activeInHierarchy) {
            CounterChangeFx fxObj = Instantiate(_changeUpFx, transform.position, Quaternion.identity, transform);
            fxObj.Init(_icon, changeAmount);
        }
    }
}