using System;
using System.Collections;
using UnityEngine;

namespace Abstract {
    public class HasAnimationAndCallback : MonoBehaviour {
        [SerializeField]
        protected Animation _animation;

        protected Action OnHideEnded;

        protected IEnumerator WaitForAnimationEnded() {
            yield return new WaitWhile(() => _animation.isPlaying);
            gameObject.SetActive(false);
            OnHideEnded?.Invoke();
        }

        protected void OnDestroy() {
            OnHideEnded = null;
        }
    }
}