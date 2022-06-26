using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviourSoundStarter {
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
        GPGSUpdated(GPSManager.isAuthenticated);

        ResetButton.SetActive(false);
    }

    public void ClosePanel() {
        gameObject.SetActive(false);
        ResetButton.SetActive(false);
    }

    public void OnNotificationChanged() {
        ResetButton.SetActive(true);
        curProfile.sendNotifications = NotificationsToggle.isOn;
        SettingsManager.instance.ChangeSettings(curProfile);
        SettingsManager.instance.NotificationChanged();
    }

    public void OnVolumeChanged() {
        ResetButton.SetActive(true);

        curProfile.masterVolume = masterSoundSlider.value;
        curProfile.musicVolume = musicSoundSlider.value;
        curProfile.effectsVolume = effectsSoundSlider.value;
        SettingsManager.instance.ChangeSettings(curProfile);
        AudioManager.instance.ChangeVolume(curProfile.masterVolume, curProfile.musicVolume, curProfile.effectsVolume);
    }

    public void OnDayMomentChanged() {
        ResetButton.SetActive(true);

        curProfile.newDayPoint = DaypointDropDown.value;
        SettingsManager.instance.ChangeSettings(curProfile);
        SettingsManager.instance.DayMomentChanged();
    }

    public void ResetChanges() {
        curProfile = UnchangedProfile;
        UpdateSettingsPanel(curProfile);
        SettingsManager.instance.ChangeSettings(curProfile);
        SettingsManager.instance.DayMomentChanged();
        SettingsManager.instance.NotificationChanged();
        AudioManager.instance.ChangeVolume(curProfile.masterVolume, curProfile.musicVolume, curProfile.effectsVolume);
    }

    public void ConnectGPGS() {
        GPSManager.instance.Initialize();
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
        SettingsManager.instance.ChangeSettings(curProfile);
        SettingsManager.instance.DayMomentChanged();
        SettingsManager.instance.NotificationChanged();
        AudioManager.instance.ChangeVolume(curProfile.masterVolume, curProfile.musicVolume, curProfile.effectsVolume);
    }
}