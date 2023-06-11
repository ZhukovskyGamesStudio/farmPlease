using System;
using DefaultNamespace.Abstract;
using DefaultNamespace.Tables;
using UnityEngine;
using UnityEngine.UI;

public class SpotlightWithText : HasAnimationAndCallback {
    [SerializeField]
    protected Text _hintText;

    [SerializeField]
    protected RectTransform _shadowCenter, _headShift;

    private const string SHOW = "SpotlightShow";
    private const string HIDE = "SpotlightHide";

    public void ShowSpotlight(Transform target, SpotlightAnimConfig animDataConfig, Action onHideEnded = null) {
        gameObject.SetActive(true);
        ShowSpotlight(target.position, animDataConfig);
        OnHideEnded = onHideEnded;
    }

    private void ShowSpotlight(Vector3 targetPos, SpotlightAnimConfig config) {
        transform.position = targetPos;
        _headShift.anchoredPosition = config.HeadShift;
        _shadowCenter.sizeDelta = config.SpotlightSize;
        _hintText.text = config.HintText;
        _animation.Play(SHOW);
    }

    public void HideSpotlight() {
        _animation.Play(HIDE);
        StartCoroutine(WaitForAnimationEnded());
    }
}