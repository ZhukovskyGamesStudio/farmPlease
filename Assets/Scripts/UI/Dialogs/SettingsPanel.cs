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
        public DevlogManager Devlog;

        [SerializeField]
        private SettingsCheatCodeView _settingsCheatCodeView;

        private SettingsData SettingsData => SaveLoadManager.CurrentSave.SettingsData;
        private SettingsData _unchangedData;

        public void Initialize(CheatCodeConfigList cheatCodeConfigList) {
            _settingsCheatCodeView.Init(cheatCodeConfigList);
            Devlog.Init();
        }

        public void UpdateSettingsPanel(SettingsData data) {
            _unchangedData = data;
            DaypointDropDown.SetValueWithoutNotify(data.NewDayPoint);
            masterSoundSlider.SetValueWithoutNotify(data.MasterVolume);
            musicSoundSlider.SetValueWithoutNotify(data.MusicVolume);
            effectsSoundSlider.SetValueWithoutNotify(data.EffectsVolume);
            NotificationsToggle.SetIsOnWithoutNotify(data.SendNotifications);
            GpgsUpdated(GpsManager.IsAuthenticated);

            ResetButton.SetActive(false);
        }

        public void ClosePanel() {
            gameObject.SetActive(false);
            ResetButton.SetActive(false);
        }

        public void OnNotificationChanged() {
            ResetButton.SetActive(true);
            SettingsData.SendNotifications = NotificationsToggle.isOn;
            Settings.Instance.ChangeSettings(SettingsData);
            Settings.Instance.NotificationChanged();
        }

        public void OnVolumeChanged() {
            ResetButton.SetActive(true);

            SettingsData.MasterVolume = masterSoundSlider.value;
            SettingsData.MusicVolume = musicSoundSlider.value;
            SettingsData.EffectsVolume = effectsSoundSlider.value;
            Settings.Instance.ChangeSettings(SettingsData);
            Audio.Instance.ChangeVolume(SettingsData.MasterVolume, SettingsData.MusicVolume, SettingsData.EffectsVolume);
        }

        public void OnDayMomentChanged() {
            ResetButton.SetActive(true);

            SettingsData.NewDayPoint = DaypointDropDown.value;
            Settings.Instance.ChangeSettings(SettingsData);
            Settings.Instance.DayMomentChanged();
        }

        public void ResetChanges() {
            SaveLoadManager.CurrentSave.SettingsData = _unchangedData;
            UpdateSettingsPanel(SettingsData);
            Settings.Instance.ChangeSettings(SettingsData);
            Settings.Instance.DayMomentChanged();
            Settings.Instance.NotificationChanged();
            Audio.Instance.ChangeVolume(SettingsData.MasterVolume, SettingsData.MusicVolume, SettingsData.EffectsVolume);
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
            SaveLoadManager.CurrentSave.SettingsData = new SettingsData();
            UpdateSettingsPanel(SettingsData);
            Settings.Instance.ChangeSettings(SettingsData);
            Settings.Instance.DayMomentChanged();
            Settings.Instance.NotificationChanged();
            Audio.Instance.ChangeVolume(SettingsData.MasterVolume, SettingsData.MusicVolume, SettingsData.EffectsVolume);
            SaveLoadManager.SaveGame();
        }
    }
}