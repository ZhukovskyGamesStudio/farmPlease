using System;
using UnityEngine;

public class AttentionView : MonoBehaviour {
    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _showClip, _idleClip, _hideClip;

    private bool _isShown;
    private bool _needPlaying;

    public void ShowAttention() {
        if (_isShown) {
            return;
        }

        _isShown = true;
        _animation.Play(_showClip.name);
        _animation.PlayQueued(_idleClip.name);
        _needPlaying = true;
    }

    private void OnEnable() {
        if (_needPlaying) {
            _animation.PlayQueued(_idleClip.name);
        }
    }

    public void Hide() {
        if (!_isShown) {
            return;
        }

        _needPlaying = false;
        _isShown = false;
        _animation.Play(_hideClip.name);
    }
}