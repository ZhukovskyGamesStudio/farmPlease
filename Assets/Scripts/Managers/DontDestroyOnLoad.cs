﻿using UnityEngine;

namespace Managers {
    public class DontDestroyOnLoad {
        private static Transform _container;

        public static Transform Container {
            get {
                if (_container == null) {
                    GameObject obj = Object.Instantiate(new GameObject());
                    obj.name = "DDOL Container";
                    Object.DontDestroyOnLoad(obj);
                    _container = obj.transform;
                }

                return _container;
            }
        }
    }
}