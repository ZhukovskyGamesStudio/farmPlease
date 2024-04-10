using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers {
    public class KeyboardManager {
        private readonly Dictionary<KeyCode, Action> _actionsMap;

        public KeyboardManager() {
            _actionsMap = new Dictionary<KeyCode, Action>() {
                {KeyCode.E, ChangeUnlimitedClockEnergy},
                {KeyCode.R, ChangeUnlimitedEnergy},

                {KeyCode.RightBracket, ChangeGameSpeedUp},
                {KeyCode.LeftBracket, ChangeGameSpeedDown},
                {KeyCode.Backslash, ChangeGameSpeedToNormal},
            };
        }

        public void CheckInputs() {
            foreach (var VARIABLE in _actionsMap) {
                if (Input.GetKeyDown(VARIABLE.Key)) {
                    VARIABLE.Value?.Invoke();
                }
            }
        }

        private void ChangeUnlimitedClockEnergy() {
            GameModeManager.Instance.Config.InfiniteClockEnergy = !GameModeManager.Instance.InfiniteClockEnergy;
        }

        private void ChangeUnlimitedEnergy() {
            GameModeManager.Instance.Config.InfiniteEnergy = !GameModeManager.Instance.InfiniteEnergy;
        }


        private void ChangeGameSpeedUp() {
            GameModeManager.Instance.Config.GameSpeed *= 2;
            if (GameModeManager.Instance.Config.GameSpeed >= 8) {
                GameModeManager.Instance.Config.GameSpeed = 8;
            }
        }

        private void ChangeGameSpeedToNormal() {
            GameModeManager.Instance.Config.GameSpeed = 1;
        }

        private void ChangeGameSpeedDown() {
            GameModeManager.Instance.Config.GameSpeed /= 2;
            if (GameModeManager.Instance.Config.GameSpeed <= 0.125f) {
                GameModeManager.Instance.Config.GameSpeed = 0.125f;
            }
        }
    }
}