using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI;
using UnityEngine;

public class FlyingCropFx : FlyingItemFx {
    [SerializeField]
    private SpriteRenderer _image;

    private Sprite _cachedSprite;
    protected override float _upShift => 0.75f;

    public void Init(Sprite icon, Vector3 parentPosition) {
        _image.sprite = icon;
        transform.position = parentPosition;
        PlayAnimAndDestroy().Forget();
    }

    public void CacheFinalSprite(Sprite icon) {
        if (_image == null) {
            _image = GetComponent<SpriteRenderer>();
        }

        _cachedSprite = icon;
        gameObject.SetActive(false);
    }

    public void SetFinalSpriteFromCache() {
        _image.sprite = _cachedSprite;
    }

    public void SetEarlyDisappear() {
        Sequence jumpSeq = DOTween.Sequence();
        jumpSeq.Join(_image.DOFade(0, 0.75f));
    }

    protected override RectTransform Target => UIHud.Instance.ShopsPanel.ScalesButton.GetComponent<RectTransform>();
}