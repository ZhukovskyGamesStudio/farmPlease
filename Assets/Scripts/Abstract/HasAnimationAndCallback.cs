using System;
using System.Collections;
using UnityEngine;

namespace Abstract {
    public class HasAnimationAndCallback : MonoBehaviour {
        [SerializeField]
        protected Animation _animation;

        protected Action OnAnimationEnded;

        protected IEnumerator WaitForAnimationEnded() {
            yield return new WaitWhile(() => _animation.isPlaying);
            gameObject.SetActive(false);
            OnAnimationEnded?.Invoke();
        }

        protected void OnDestroy() {
            OnAnimationEnded = null;
        }
    }
}