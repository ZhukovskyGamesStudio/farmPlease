using Managers;
using UnityEngine;
using UnityEngine.UI;

public class ProfileView : MonoBehaviour {
    [SerializeField]
    private Slider _slider;

    [field: SerializeField]
    public AnimatableProgressbar XpProgressBar { get; private set; }


    [SerializeField]
    private Image _levelIcon;

    public bool IsLockedByFtue;

    public void SetData(GameSaveProfile profile) {
        int curLevelMin = XpUtils.XpByLevel(profile.CurrentLevel);
        int nextLevel = XpUtils.XpByLevel(profile.CurrentLevel + 1);
        XpProgressBar.SetAmount(profile.Xp - curLevelMin, nextLevel - curLevelMin);
        _levelIcon.sprite = ConfigsManager.Instance.LevelsConfig.LevelConfigs[profile.CurrentLevel].LevelMiniIcon;
    }

    public void OnClick() {
        if (IsLockedByFtue) {
            return;
        }

        DialogsManager.Instance.ShowDialogWithData(typeof(ProfileDialog), new ProfileDialogData() {
            ReshowProfileView = Show
        });
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}