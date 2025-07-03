using System;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingStatePanel : MonoBehaviour {
    [SerializeField]
    private Image _faceImage;

    [SerializeField]
    private Sprite _yesSprite, _noSprite;

    [SerializeField]
    private TextMeshProUGUI _bubbleText;

    [SerializeField]
    [LocalizationKey("Main")]
    private string _moveLoc, _unableLoc, _canPlaceLoc;

    bool _canPlace = false;
    private Action _onDrag, _onPlace;
    
    public void Activate(Action onDrag, Action onPlace) {
        _onPlace = onPlace;
        _onDrag = onDrag;
        gameObject.SetActive(true);
        SetFace(false);
        _bubbleText.text = LocalizationUtils.L(_moveLoc);
    }

    public void SetState(bool canPlace) {
        _bubbleText.text = LocalizationUtils.L(canPlace ? _canPlaceLoc : _unableLoc);
        _canPlace = canPlace;
        SetFace(canPlace);
    }

    private void SetFace(bool isYes) {
        _faceImage.sprite = isYes ? _yesSprite : _noSprite;
    }

    public void Drag() {
        _onDrag?.Invoke();
    }

    public void ApplyPosition() {
        if (!_canPlace) {
            return;
        }
        _onPlace?.Invoke();
    }
}