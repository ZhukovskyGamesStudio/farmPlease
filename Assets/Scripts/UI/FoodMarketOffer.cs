using System;
using UnityEngine;
using UnityEngine.UI;

public class FoodMarketOffer : MonoBehaviour {
    [SerializeField]
    private Image _image, _boughtImage;

    [SerializeField]
    private Toggle _toggle;

    private Action _onButtonClick;

    public void Init(Sprite icon, Action onButtonClick) {
        _image.sprite = icon;
        _onButtonClick = onButtonClick;
    }

    public void OfferButtonClicked(bool isOn) {
        if (!isOn) {
            return;
        }

        _onButtonClick?.Invoke();
    }

    public void UpdateInteractable(bool isInteractable) {
        _boughtImage.gameObject.SetActive(!isInteractable);
    }
}