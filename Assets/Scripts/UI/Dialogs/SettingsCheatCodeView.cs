using System;
using Localization;
using Managers;
using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace UI {
    public class SettingsCheatCodeView : MonoBehaviour {
        [SerializeField]
        private TMP_InputField _cheatCodeInput;

        [SerializeField]
        private TextMeshProUGUI _cheatCodeErrorText;

        private CheatCodeManager _cheatCodeManager;

        public void Init(CheatCodeConfigList cheatCodeConfigList) {
            _cheatCodeManager = new CheatCodeManager(cheatCodeConfigList);
            LocalizationManager.Instance.OnLanguageChanged += (s) => _cheatCodeErrorText.text = "";
        }

        private void OnEnable() {
            _cheatCodeInput.SetTextWithoutNotify("");
            _cheatCodeErrorText.text = "";
        }

        public void InputCheatcode(string code) {
            code = code.ToUpper();
            bool isSuccess = _cheatCodeManager.CheatCodeAvailable(code, out string errorMessage);
            if (isSuccess) {
                _cheatCodeManager.ExecuteCheatCode(code);
                _cheatCodeInput.SetTextWithoutNotify("");
                _cheatCodeErrorText.text = LocalizationUtils.L("code_success");
            } else {
                _cheatCodeErrorText.text = errorMessage;
            }
        }
    }
}