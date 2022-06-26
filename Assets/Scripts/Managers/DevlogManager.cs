using UnityEngine;
using UnityEngine.UI;

public class DevlogManager : MonoBehaviour {
    public DevlogSO DevlogSo;
    public Text Header, MainText;

    public void Init() {
        Header.text = DevlogSo.Header;
        MainText.text = DevlogSo.MainText;
    }
}