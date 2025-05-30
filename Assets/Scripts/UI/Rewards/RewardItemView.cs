using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _nameText, _amountText;

    [SerializeField]
    private Image _icon;

    public void SetData(Sprite sprite, string rewardName) {
        _icon.sprite = sprite;
        _nameText.text = rewardName;
        _amountText.gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite, int amount) {
        _icon.sprite = sprite;
        _nameText.gameObject.SetActive(false);
        _amountText.text = amount.ToString();
    }
}