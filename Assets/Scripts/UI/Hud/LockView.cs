using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _lvlToUnlock;

    [SerializeField]
    private Button _lockedButton;

    private int _levelToUnlock;

    public void SetLevelToUnlock(int level) {
        _levelToUnlock = level;
        _lvlToUnlock.text = level <= 1 ? "?" : level.ToString();
    }

    public void SetInteractable(bool isUnlocked) {
        _lockedButton.interactable = isUnlocked;
        gameObject.SetActive(!isUnlocked);
    }

    public void Unlock() {
        _lockedButton.interactable = true;
        gameObject.SetActive(false);
    }
}