using TMPro;
using UnityEngine;

namespace UI {
    public class EnergyRefillDialog : DialogWithData<string> {
        [SerializeField]
        private TextMeshProUGUI _hintText;

        public override void SetData(string data) {
            _hintText.text = data;
        }
    }
}