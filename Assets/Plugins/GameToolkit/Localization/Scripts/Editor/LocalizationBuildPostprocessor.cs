// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace GameToolkit.Localization.Editor {
    public class LocalizationBuildPostprocessor {
        [PostProcessBuild(9999)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject) {
#if UNITY_IOS
            if (buildTarget == BuildTarget.iOS)
            {
                // Continue if any localization info exists.
                var localizations = GetLocalizations();
                if (localizations.Count == 0)
                {
                    return;
                }

                // Get plist.
                var plistPath = pathToBuiltProject + "/Info.plist";
                var plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));

                // Get root of plist/dict.
                var rootDict = plist.root;
                var plistLocalizations = rootDict.CreateArray("CFBundleLocalizations");

                // Add localizations.
                foreach (string locale in localizations)
                {
                    plistLocalizations.AddString(locale);
                    Debug.Log("[LocalizationBuildPostprocessor] Localization added: " + locale);
                }

                // Save all changes.
                File.WriteAllText(plistPath, plist.WriteToString());
            }
#endif
        }

        private static List<string> GetLocalizations() {
            List<string> localizations = new List<string>();
            LocalizationSettings localizationSettings = LocalizationSettings.Instance;
            if (localizationSettings)
                foreach (SystemLanguage language in localizationSettings.AvailableLanguages)
                    localizations.Add(Localization.GetLanguageCode(language));
            return localizations;
        }
    }
}