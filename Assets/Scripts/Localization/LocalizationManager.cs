using UnityEngine;
using System;
using System.Collections.Generic;
using Abstract;
using Managers;
using Debug = UnityEngine.Debug;

namespace Localization {
    public class LocalizationManager : PreloadableSingleton<LocalizationManager> {
        [Header("Localization Settings")]
        [SerializeField]
        private LocalizationData[] _localizationFiles;

        public event Action<string> OnLanguageChanged;

        public override int InitPriority => 1;

        public string CurrentLanguage {
            get => SaveLoadManager.CurrentSave.CurrentLanguage;
            set {
                if (SaveLoadManager.CurrentSave.CurrentLanguage != value) {
                    SaveLoadManager.CurrentSave.CurrentLanguage = value;
                    OnLanguageChanged?.Invoke(SaveLoadManager.CurrentSave.CurrentLanguage);
                }
            }
        }

        // Публичное свойство для доступа к данным локализации
        public LocalizationData[] LocalizationFiles => _localizationFiles;

        protected override void OnFirstInit() {
            base.OnFirstInit();
            InitializeLocalization();
        }

        private void InitializeLocalization() {
            if (_localizationFiles != null && _localizationFiles.Length > 0) {
                foreach (var file in _localizationFiles) {
                    if (file != null) file.Initialize();
                }
            } else {
                Debug.LogError("LocalizationFiles не назначены в LocalizationManager!");
            }
        }

        public string GetText(string key) {
            if (_localizationFiles == null || _localizationFiles.Length == 0) {
                Debug.LogError("LocalizationFiles не назначены!");
                return key;
            }

            foreach (var file in _localizationFiles) {
                if (file != null && file.HasKey(key))
                    return file.GetText(key, SaveLoadManager.CurrentSave.CurrentLanguage);
            }

            Debug.LogWarning($"Localization key not found: {key}");
            return key;
        }

        public string GetText(string key, string language) {
            if (_localizationFiles == null || _localizationFiles.Length == 0) {
                Debug.LogError("LocalizationFiles не назначены!");
                return key;
            }

            foreach (var file in _localizationFiles) {
                if (file != null && file.HasKey(key))
                    return file.GetText(key, language);
            }

            Debug.LogWarning($"Localization key not found: {key}");
            return key;
        }

        public bool HasKey(string key) {
            if (_localizationFiles == null) return false;
            foreach (var file in _localizationFiles) {
                if (file != null && file.HasKey(key)) return true;
            }

            return false;
        }

        public void SetLanguage(string language) {
            CurrentLanguage = language;
        }

        public void SetLanguageToRussian() {
            SetLanguage("ru");
        }

        public void SetLanguageToEnglish() {
            SetLanguage("en");
        }

        public void ChangeLanguageToNext() {
            int cur = Languages.IndexOf(CurrentLanguage);
            cur++;
            cur %= Languages.Count;
            SetLanguage(Languages[cur]);
        }

        private readonly List<string> Languages = new List<string>() {
            "en",
            "ru"
        };

       
    }
}