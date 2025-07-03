using System;
using Localization;
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
    private TextMeshProUGUI _costText, _boughtText;

    [SerializeField]
    private Image _confirmImage;

    [SerializeField]
    private GameObject _boughtState, _unboughtState;

    [SerializeField]
    [LocalizationKey("BuildingShop")]
    private string _buildingShopBuy, _buildingShopOpen, _buildingShopBought, _buildingShopOpened;

    private Action _onConfirmed;

    public void SetData(Sprite sprite, string headerText, string explainText, int cropsCost, Action onConfirmed, Color headerColor,bool isBuilding = false) {
        _confirmImage.sprite = sprite;
        _nameText.text = headerText;
        _nameText.color = headerColor;
        _explanationText.text = explainText;
        _costText.text = (isBuilding ? $"{LocalizationUtils.L(_buildingShopBuy)} " : $"{LocalizationUtils.L(_buildingShopOpen)} ") + cropsCost;
        _boughtText.text = isBuilding ? LocalizationUtils.L(_buildingShopBought): LocalizationUtils.L(_buildingShopOpened);
        _onConfirmed = onConfirmed;
    }

    public void ConfirmButton() {
        _onConfirmed?.Invoke();
    }

    public void UpdateInteractable(bool isBought,bool isInteractable) {
        _confirmButton.interactable = isInteractable;
        _boughtState.SetActive(isBought);
        _unboughtState.SetActive(!isBought);
    }
}