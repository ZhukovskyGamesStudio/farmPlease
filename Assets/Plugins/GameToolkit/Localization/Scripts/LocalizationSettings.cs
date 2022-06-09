// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameToolkit.Localization {
    [CreateAssetMenu(fileName = "LocalizationSettings", menuName = "GameToolkit/Localization/Localization Settings",
        order = 0)]
    public sealed class LocalizationSettings : ScriptableObject {
        private const string AssetName = "LocalizationSettings";
        private static LocalizationSettings s_Instance;

        [SerializeField]
        [Tooltip("Enabled languages for the application.")]
        private List<SystemLanguage> m_AvailableLanguages = new(1) {
            SystemLanguage.English
        };

        [Tooltip("Google Cloud authentication file.")]
        public TextAsset GoogleAuthenticationFile;

        /// <summary>
        ///     Gets the localization settings instance.
        /// </summary>
        public static LocalizationSettings Instance {
            get {
                if (!s_Instance) s_Instance = FindByResources();

#if UNITY_EDITOR
                if (!s_Instance) s_Instance = CreateSettingsAndSave();
#endif

                if (!s_Instance) {
                    Debug.LogWarning("No instance of " + AssetName + " found, using default values.");
                    s_Instance = CreateInstance<LocalizationSettings>();
                }

                return s_Instance;
            }
        }

        /// <summary>
        ///     Enabled languages for the application.
        /// </summary>
        public List<SystemLanguage> AvailableLanguages => m_AvailableLanguages;

        private static LocalizationSettings FindByResources() {
            return Resources.Load<LocalizationSettings>(AssetName);
        }

#if UNITY_EDITOR
        private static LocalizationSettings CreateSettingsAndSave() {
            LocalizationSettings localizationSettings = CreateInstance<LocalizationSettings>();

            // Saving during Awake() will crash Unity, delay saving until next editor frame.
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                EditorApplication.delayCall += () => SaveAsset(localizationSettings);
            else
                SaveAsset(localizationSettings);

            return localizationSettings;
        }

        private static void SaveAsset(LocalizationSettings localizationSettings) {
            string assetPath = "Assets/Resources/" + AssetName + ".asset";
            string directoryName = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);
            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(localizationSettings, uniqueAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(AssetName + " has been created: " + assetPath);
        }
#endif
    }
}