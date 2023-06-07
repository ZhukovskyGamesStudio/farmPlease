using System;
using UnityEngine;

namespace DefaultNamespace.Abstract {
    public class Singleton<T> : SingletonBase<T> where T : CustomMonoBehaviour {
        private void Awake() {
            CreateSingleton();
        }
    }
}