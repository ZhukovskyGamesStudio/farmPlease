using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainQuestLockedView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _headerText;

    [SerializeField]
    private Slider _slider;

    public void SetData(int nextLevel,  float percent) {
        _headerText.text = "достигнете уровня <color=#2CE2EF>%N</color>\nчтобы открыть задание".Replace("%N", nextLevel.ToString());
        _slider.value = percent;
    }
}