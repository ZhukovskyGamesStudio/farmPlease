using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodMarketConfirmDialog : MonoBehaviour {
    [SerializeField]
    private Button _confirmButton;

    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private TextMeshProUGUI _explanationText;

    [SerializeField]
    private TextMeshProUGUI _costText;

    [SerializeField]
    private Image _confirmImage;

    private Action _onConfirmed;

    public void SetData(Sprite sprite, string headerText, string explainText, int cropsCost, Action onConfirmed) {
        _confirmImage.sprite = sprite;
        _nameText.text = headerText;
        _explanationText.text = explainText;
        _costText.text = "Открыть за " + cropsCost;
        _onConfirmed = onConfirmed;
    }

    public void ConfirmButton() {
        _onConfirmed?.Invoke();
    }

    public void UpdateInteractable(bool isInteractable) {
        _confirmButton.interactable = isInteractable;
    }
}