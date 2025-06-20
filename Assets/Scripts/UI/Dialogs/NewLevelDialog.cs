﻿using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class NewLevelDialog : DialogWithData<int> {
    [SerializeField]
    private Image _previousLevelIcon, _nextLevelIcon;

    [SerializeField]
    private TextMeshProUGUI _previousLevelName, _nextLevelName;

    [SerializeField]
    private Animation _levelAnimation;

    [SerializeField]
    private AnimationClip _previousIdleClip, _clickOnLevelClip, _levelChangeClip, _nextIdleClip;

    private int _clicksNeeded, _clicksMade;

    public override void SetData(int newLevel) {
        _previousLevelIcon.sprite = ConfigsManager.Instance.LevelsIcon[newLevel - 1];
        _nextLevelIcon.sprite = ConfigsManager.Instance.LevelsIcon[newLevel];

        _previousLevelName.text = ConfigsManager.Instance.LevelConfigs[newLevel - 1].LevelName;
        _nextLevelName.text = ConfigsManager.Instance.LevelConfigs[newLevel].LevelName;

        _clicksNeeded = 3 + newLevel;
    }

    public override async UniTask Show(Action onClose) {
        UIHud.Instance.ProfileView.Hide();
        await base.Show(onClose);
        _levelAnimation.Play(_previousIdleClip.name);
    }

    protected override UniTask Close() {
        UIHud.Instance.ProfileView.Show();
        return base.Close();
    }

    public void Click() {
        if (_clicksMade >= _clicksNeeded) {
            return;
        }

        _clicksMade++;

        if (_clicksMade >= _clicksNeeded) {
            ChangeLevel();
        } else {
            _levelAnimation.Play(_clickOnLevelClip.name);
            _levelAnimation.PlayQueued(_previousIdleClip.name);
        }
    }

    private void ChangeLevel() {
        _levelAnimation.Play(_clickOnLevelClip.name);
        _levelAnimation.PlayQueued(_levelChangeClip.name);
        _levelAnimation.PlayQueued(_nextIdleClip.name);
    }
}