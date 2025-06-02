using UnityEngine;

public class AttentionView : MonoBehaviour {
    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _showClip, _idleClip, _hideClip;

    private bool _isShown;

    public void ShowAttention() {
        if (_isShown) {
            return;
        }

        _animation.Play(_showClip.name);
        _animation.PlayQueued(_idleClip.name);
    }

    public void Hide() {
        _animation.Play(_hideClip.name);
    }
}