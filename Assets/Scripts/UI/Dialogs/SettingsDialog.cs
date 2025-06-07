using Abstract;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class SettingsDialog : DialogWithData<CheatCodeConfigList>, ISoundStarter {
        public Slider masterSoundSlider, musicSoundSlider, effectsSoundSlider;
        public Button GPGSButton;
        public Text GPGSText;
        public Toggle NotificationsToggle;
        public GameObject ResetButton;
        public DevlogManager Devlog;

        [SerializeField]
        private SettingsCheatCodeView _settingsCheatCodeView;

        private SettingsData SettingsData => SaveLoadManager.CurrentSave.SettingsData;
        private SettingsData _unchangedData;

        public override void SetData(CheatCodeConfigList data) {
            Initialize(data);
            UpdateSettingsPanel(SettingsData);
        }

        public void Initialize(CheatCodeConfigList cheatCodeConfigList) {
            _settingsCheatCodeView.Init(cheatCodeConfigList);
            Devlog.Init();
        }

        public void UpdateSettingsPanel(SettingsData data) {
            _unchangedData = data;
            masterSoundSlider.SetValueWithoutNotify(data.MasterVolume);
            musicSoundSlider.SetValueWithoutNotify(data.MusicVolume);
            effectsSoundSlider.SetValueWithoutNotify(data.EffectsVolume);
            NotificationsToggle.SetIsOnWithoutNotify(data.SendNotifications);
            GpgsUpdated(GpsManager.IsAuthenticated);

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

        public void ResetChanges() {
            SaveLoadManager.CurrentSave.SettingsData = _unchangedData;
            UpdateSettingsPanel(SettingsData);
            Settings.Instance.ChangeSettings(SettingsData);
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

        public void ShowDevlog() {
            Devlog.gameObject.SetActive(true);
        }

        public void ClearSettings() {
            SaveLoadManager.CurrentSave.SettingsData = new SettingsData();
            UpdateSettingsPanel(SettingsData);
            Settings.Instance.ChangeSettings(SettingsData);
            Settings.Instance.NotificationChanged();
            Audio.Instance.ChangeVolume(SettingsData.MasterVolume, SettingsData.MusicVolume, SettingsData.EffectsVolume);
            SaveLoadManager.SaveGame();
        }

        public void ClearSave() {
            PlayerPrefs.DeleteAll();
            Application.Quit();
        }
    }
}