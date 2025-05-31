using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _nameText, _amountText;

    [SerializeField]
    private BackpackItemBackView _backView;
    
    [SerializeField]
    private Image _icon;

    public void SetData(Sprite sprite, string rewardName, ItemColorType colorType) {
        _icon.sprite = sprite;
        _nameText.text = rewardName;
        _amountText.gameObject.SetActive(false);
        _backView.InitColor(colorType);
        gameObject.SetActive(true);
    }

    public void SetData(Sprite sprite, int amount, ItemColorType colorType) {
        _icon.sprite = sprite;
        _nameText.gameObject.SetActive(false);
        _amountText.text = amount.ToString();
        _backView.InitColor(colorType);
        gameObject.SetActive(true);
    }
}