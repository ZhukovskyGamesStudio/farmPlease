using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Managers {
    public class DevlogManager : MonoBehaviour {
        public DevlogConfig devlogConfig;
        public TextMeshProUGUI Header, MainText;

        public void Init() {
            Header.text = devlogConfig.Header;
            MainText.text = devlogConfig.MainText;
        }
    }
}