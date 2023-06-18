using Abstract;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
    public class SettingsPanel : MonoBehaviour, ISoundStarter {
        public Slider masterSoundSlider, musicSoundSlider, effectsSoundSlider;
        public Button GPGSButton;
        public Text GPGSText;
        public Toggle NotificationsToggle;
        public Dropdown DaypointDropDown;
        public GameObject ResetButton;
        public bool IsMainMenu;
        public DevlogManager Devlog;

        [SerializeField] private SettingsCheatCodeView _settingsCheatCodeView;

        private SettingsProfile _curProfile, _unchangedProfile;

        public void Initialize(CheatCodeConfigList cheatCodeConfigList) {
            _curProfile = new SettingsProfile();
            _settingsCheatCodeView.Init(cheatCodeConfigList);
            Devlog.Init();
        }

        public void UpdateSettingsPanel(SettingsProfile profile) {
            _curProfile = new SettingsProfile(profile);
            _unchangedProfile = profile;
            DaypointDropDown.SetValueWithoutNotify(profile.NewDayPoint);
            masterSoundSlider.SetValueWithoutNotify(profile.MasterVolume);
            musicSoundSlider.SetValueWithoutNotify(profile.MusicVolume);
            effectsSoundSlider.SetValueWithoutNotify(profile.EffectsVolume);
            NotificationsToggle.SetIsOnWithoutNotify(profile.SendNotifications);
            GpgsUpdated(GpsManager.IsAuthenticated);

            ResetButton.SetActive(false);
        }

        public void ClosePanel() {
            gameObject.SetActive(false);
            ResetButton.SetActive(false);
        }

        public void OnNotificationChanged() {
            ResetButton.SetActive(true);
            _curProfile.SendNotifications = NotificationsToggle.isOn;
            Settings.Instance.ChangeSettings(_curProfile);
            Settings.Instance.NotificationChanged();
        }

        public void OnVolumeChanged() {
            ResetButton.SetActive(true);

            _curProfile.MasterVolume = masterSoundSlider.value;
            _curProfile.MusicVolume = musicSoundSlider.value;
            _curProfile.EffectsVolume = effectsSoundSlider.value;
            Settings.Instance.ChangeSettings(_curProfile);
            Audio.Instance.ChangeVolume(_curProfile.MasterVolume, _curProfile.MusicVolume, _curProfile.EffectsVolume);
        }

        public void OnDayMomentChanged() {
            ResetButton.SetActive(true);

            _curProfile.NewDayPoint = DaypointDropDown.value;
            Settings.Instance.ChangeSettings(_curProfile);
            Settings.Instance.DayMomentChanged();
        }

        public void ResetChanges() {
            _curProfile = _unchangedProfile;
            UpdateSettingsPanel(_curProfile);
            Settings.Instance.ChangeSettings(_curProfile);
            Settings.Instance.DayMomentChanged();
            Settings.Instance.NotificationChanged();
            Audio.Instance.ChangeVolume(_curProfile.MasterVolume, _curProfile.MusicVolume, _curProfile.EffectsVolume);
        }

        public void ConnectGpgs() {
            GpsManager.Instance.Initialize();
        }

        public void GpgsUpdated(bool isAuthenticated) {
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
            _curProfile.Clear();
            _curProfile.Load();
            UpdateSettingsPanel(_curProfile);
            Settings.Instance.ChangeSettings(_curProfile);
            Settings.Instance.DayMomentChanged();
            Settings.Instance.NotificationChanged();
            Audio.Instance.ChangeVolume(_curProfile.MasterVolume, _curProfile.MusicVolume, _curProfile.EffectsVolume);
        }
    }
}