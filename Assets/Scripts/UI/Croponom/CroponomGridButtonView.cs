using System;
using UnityEngine;
using UnityEngine.UI;

public class CroponomGridButtonView : MonoBehaviour {
    [SerializeField]
    private Image _image;

    [SerializeField]
    private Button _button;

    private ConfigWithCroponomPage _config;
    private Action<ConfigWithCroponomPage> _onClick;

    public void SetData(ConfigWithCroponomPage config, Action<ConfigWithCroponomPage> onClick) {
        _config = config;
        _onClick = onClick;
    }

    public void SetLockState(bool isUnlocked) {
        _image.sprite = isUnlocked ? _config.gridIcon : _config.LockedGridIcon;
        _button.interactable = isUnlocked;
    }

    public void OpenPage() {
        _onClick?.Invoke(_config);
    }

    public string GetUnlockable() => _config.GetUnlockable();
}