using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class DevlogManager : MonoBehaviour {
        public DevlogConfig devlogConfig;
        public Text Header, MainText;

        public void Init() {
            Header.text = devlogConfig.Header;
            MainText.text = devlogConfig.MainText;
        }
    }
}