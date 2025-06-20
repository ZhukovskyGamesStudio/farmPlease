using UnityEngine;
using UnityEngine.UI;

public class FastPanelSlotView : MonoBehaviour {

    [SerializeField]
    private Image _backImage, _toolImage;
    
    [SerializeField]
    private Sprite _normalSprite, _pressedSprite;
    
    public void SetToolImage(Sprite sprite) {
        _toolImage.sprite = sprite;
    }
    
    public void SetIsPressed(bool isPressed) {
        _backImage.sprite = isPressed ? _pressedSprite : _normalSprite;
    }
}