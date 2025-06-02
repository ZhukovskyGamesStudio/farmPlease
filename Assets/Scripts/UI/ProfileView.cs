using Managers;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ProfileView : MonoBehaviour {
    [SerializeField]
    private Slider _slider;

    [field: SerializeField]
    public AnimatableProgressbar XpProgressBar { get; private set; }

    public bool IsLockedByFtue;

    public void SetCounters() {
        int curLevelMin = XpUtils.XpByLevel(SaveLoadManager.CurrentSave.CurrentLevel);
        int nextLevel = XpUtils.XpByLevel(SaveLoadManager.CurrentSave.CurrentLevel + 1);
        XpProgressBar.SetAmount(SaveLoadManager.CurrentSave.Xp - curLevelMin, nextLevel - curLevelMin);
    }

    public void OnClick() {
        if (IsLockedByFtue) {
            return;
        }

        DialogsManager.Instance.ShowDialog(typeof(ProfileDialog), () => { });
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}