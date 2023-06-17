using UnityEngine;

namespace DefaultNamespace.Abstract {
    public abstract class SingletonBase<T> : CustomMonoBehaviour where T : CustomMonoBehaviour {
        public static T Instance => _instance as T;

        protected static CustomMonoBehaviour _instance;

        protected void CreateSingleton() {
            if (_instance == null) {
                _instance = this;
                OnFirstInit();
            } else if (_instance != this) {
                Destroy(gameObject);
            }
        }

        protected virtual void OnFirstInit() {
        }
    }
}