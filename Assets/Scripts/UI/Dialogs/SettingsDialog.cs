using System;
using Abstract;
using Cysharp.Threading.Tasks;
using Localization;
using Managers;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZhukovskyGamesPlugin;

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

        [SerializeField]
        private TextMeshProUGUI _versionText;

        [SerializeField]
        private Image _countryImage;
        
        [SerializeField]
        
        private SerializableDictionary<string, Sprite> _languageSprites;
        
        
        private SettingsData SettingsData => SaveLoadManager.CurrentSave.SettingsData;
        private SettingsData _unchangedData;

        public override UniTask Show(Action onClose) {
            UIHud.Instance.ProfileView.Hide();
            _countryImage.sprite = _languageSprites[LocalizationManager.Instance.CurrentLanguage];
            return base.Show(onClose);
        }

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
            _versionText.text = "v"+Application.version;
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

        protected override async UniTask Close() {
            await base.Close();
            UIHud.Instance.ProfileView.Show();
        }

        public void ChangeLanguage() {
            LocalizationManager.Instance.ChangeLanguageToNext();
            _countryImage.sprite = _languageSprites[LocalizationManager.Instance.CurrentLanguage];
            SaveLoadManager.SaveGame();
        }
    }
}