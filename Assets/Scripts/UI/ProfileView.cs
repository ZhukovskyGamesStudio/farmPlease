using Managers;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ProfileView : MonoBehaviour {
    [SerializeField]
    private Slider _slider;

    [field: SerializeField]
    public AnimatableProgressbar XpProgressBar { get; private set; }

    public void SetCounters() {
        XpProgressBar.SetAmount(SaveLoadManager.CurrentSave.Xp, XpUtils.GetNextLevelByXp(SaveLoadManager.CurrentSave.Xp));
    }

    public void OnClick() {
        UIHud.Instance.ProfileDialog.Show();
    }
}