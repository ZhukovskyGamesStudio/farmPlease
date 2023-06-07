using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour,ISoundStarter {
    public Slider masterSoundSlider, musicSoundSlider, effectsSoundSlider;
    public Button GPGSButton;
    public Text GPGSText;
    public Toggle NotificationsToggle;
    public Dropdown DaypointDropDown;
    public GameObject ResetButton;
    public bool IsMainMenu;
    public DevlogManager Devlog;
 

    private SettingsProfile curProfile, UnchangedProfile;

    public void Initialize() {
        curProfile = new SettingsProfile();
        Devlog.Init();
    }

    public void UpdateSettingsPanel(SettingsProfile profile) {
        curProfile = new SettingsProfile(profile);
        UnchangedProfile = profile;
        DaypointDropDown.SetValueWithoutNotify(profile.newDayPoint);
        masterSoundSlider.SetValueWithoutNotify(profile.masterVolume);
        musicSoundSlider.SetValueWithoutNotify(profile.musicVolume);
        effectsSoundSlider.SetValueWithoutNotify(profile.effectsVolume);
        NotificationsToggle.SetIsOnWithoutNotify(profile.sendNotifications);
        GPGSUpdated(Gps.isAuthenticated);

        ResetButton.SetActive(false);
    }

    public void ClosePanel() {
        gameObject.SetActive(false);
        ResetButton.SetActive(false);
    }

    public void OnNotificationChanged() {
        ResetButton.SetActive(true);
        curProfile.sendNotifications = NotificationsToggle.isOn;
        Settings.Instance.ChangeSettings(curProfile);
        Settings.Instance.NotificationChanged();
    }

    public void OnVolumeChanged() {
        ResetButton.SetActive(true);

        curProfile.masterVolume = masterSoundSlider.value;
        curProfile.musicVolume = musicSoundSlider.value;
        curProfile.effectsVolume = effectsSoundSlider.value;
        Settings.Instance.ChangeSettings(curProfile);
        Audio.Instance.ChangeVolume(curProfile.masterVolume, curProfile.musicVolume, curProfile.effectsVolume);
    }

    public void OnDayMomentChanged() {
        ResetButton.SetActive(true);

        curProfile.newDayPoint = DaypointDropDown.value;
        Settings.Instance.ChangeSettings(curProfile);
        Settings.Instance.DayMomentChanged();
    }

    public void ResetChanges() {
        curProfile = UnchangedProfile;
        UpdateSettingsPanel(curProfile);
        Settings.Instance.ChangeSettings(curProfile);
        Settings.Instance.DayMomentChanged();
        Settings.Instance.NotificationChanged();
        Audio.Instance.ChangeVolume(curProfile.masterVolume, curProfile.musicVolume, curProfile.effectsVolume);
    }

    public void ConnectGPGS() {
        Gps.Instance.Initialize();
    }

    public void GPGSUpdated(bool isAuthenticated) {
        if (isAuthenticated) {
            GPGSButton.interactable = false;
            GPGSText.text = "Play Games подключены";
        }
    }

    public void StartTutorial() {
        SceneManager.LoadScene("Training");
        SceneManager.sceneLoaded += ClosePanel;
    }

    private void ClosePanel(Scene arg0, LoadSceneMode arg1) {
        SceneManager.sceneLoaded -= ClosePanel;
        ClosePanel();
    }

    public void ShowDevlog() {
        Devlog.gameObject.SetActive(true);
    }

    public void ClearSettings() {
        curProfile.Clear();
        curProfile.Load();
        UpdateSettingsPanel(curProfile);
        Settings.Instance.ChangeSettings(curProfile);
        Settings.Instance.DayMomentChanged();
        Settings.Instance.NotificationChanged();
        Audio.Instance.ChangeVolume(curProfile.masterVolume, curProfile.musicVolume, curProfile.effectsVolume);
    }
}