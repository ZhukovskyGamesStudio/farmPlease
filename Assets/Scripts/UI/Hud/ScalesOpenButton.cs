using UnityEngine;

public class ScalesOpenButton : MonoBehaviour {
    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _addedClip;

    public void PlayAddedAnimation() {
        _animation.Play(_addedClip.name);
    }
}