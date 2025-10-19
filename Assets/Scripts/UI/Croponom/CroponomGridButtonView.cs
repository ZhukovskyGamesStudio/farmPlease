using System;
using UnityEngine;
using UnityEngine.UI;

public class CroponomGridButtonView : MonoBehaviour {
    [SerializeField]
    private Image _icon, _lockedIcon;

    [SerializeField]
    private Image _backImage;

    [SerializeField]
    private Sprite _backLockSprite, _backUnlockSprite;

    [SerializeField]
    private Button _button;

    [SerializeField]
    private AttentionView _attentionView;

    [SerializeField]
    private LockView _lockView;

    private ConfigWithCroponomPage _config;
    private Action<ConfigWithCroponomPage> _onClick;

    public void SetData(ConfigWithCroponomPage config, Action<ConfigWithCroponomPage> onClick) {
        _config = config;
        _onClick = onClick;
    }

    public void SetLockState(bool isUnlocked) {
        _backImage.sprite = isUnlocked ? _backUnlockSprite : _backLockSprite;
        _icon.sprite = _config.gridIcon;
        _icon.SetNativeSize();
        _icon.gameObject.SetActive(isUnlocked);
        _lockedIcon.sprite = _config.LockedGridIcon;
        _lockedIcon.SetNativeSize();
        _lockedIcon.gameObject.SetActive(!isUnlocked);
        _button.interactable = isUnlocked;
        int lvlToUnlock = UnlockableUtils.FindUnlockLvl(_config.GetUnlockable());
        _lockView.SetLevelToUnlock(lvlToUnlock + 1);
        _lockView.SetInteractable(isUnlocked);
    }

    public void SetAttentionState(bool isAttention) {
        _attentionView.gameObject.SetActive(isAttention);
        if (isAttention) {
            _attentionView.ShowAttention();
        } else {
            _attentionView.Hide();
        }
    }

    public void OpenPage() {
        _onClick?.Invoke(_config);
        UnlockableUtils.TryRemoveSeenPage(_config.GetUnlockable());
        _attentionView.Hide();
    }

    public string GetUnlockable() => _config.GetUnlockable();
}