using System.Collections;
using UI;
using UnityEngine;

public class FlyingCropFx : FlyingItemFx {
    [SerializeField]
    private SpriteRenderer _image;

    public void Init(Sprite icon, Vector3 parentPosition) {
        _image.sprite = icon;
        transform.position = parentPosition;
        StartCoroutine(PlayAnimAndDestroy());
    }

    protected override RectTransform Target => UIHud.Instance.ShopsPanel.ScalesButton.GetComponent<RectTransform>();
}