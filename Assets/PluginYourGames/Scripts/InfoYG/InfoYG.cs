﻿#if YG_PLATFORM
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using YG.Insides;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
#endif

namespace YG
{
    public partial class InfoYG : ScriptableObject
    {
        public static InfoYG instance;
        public static InfoYG Inst()
        {
            if (instance == null)
            {
                InfoYG infoRes = Resources.Load<InfoYG>(NAME_INFOYG_FILE);

#if UNITY_EDITOR
                if (infoRes == null)
                {
                    InfoYG infoYG = ScriptableObject.CreateInstance<InfoYG>();
                    string path = $"{PATCH_ASSETS_YG2}/Resources/{NAME_INFOYG_FILE}.asset";
                    string directory = $"{PATCH_ASSETS_YG2}/Resources";

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    AssetDatabase.CreateAsset(infoYG, path);
                    AssetDatabase.Refresh();
                    infoRes = Resources.Load<InfoYG>(NAME_INFOYG_FILE);

                    instance = infoRes;
#if PLATFORM_WEBGL
                    if (EditorUtility.DisplayDialog($"Optimal settings",
                        "Выставить оптимальные настройки проекта и плагина для платформы по умолчанию «Яндекс Игры»? (Рекомендуется)\n\nSet the optimal project and plugin settings for the default platform «Yandex Games» platform? (Recommended)",
                        "Yes",
                        "No"))
                    {
                        instance.Basic.autoApplySettings = true;
                        SetDefaultPlatform();
                    }
                    else
                    {
                        NullPlatform();
                    }
#else
                    EditorUtility.DisplayDialog($"Optimal settings",
                        "В настройках билда не выбрана платформа WebGL. Оптимальные настройки для стандартной платформы «Яндекс Игры» не будут установлены.\nЧтобы их установить: смените платформу на WebGL, в настройках плагина включите опцию Auto Apply Settings и переключите платформу в поле Platforms.\n\nThe WebGL platform is not selected in the build settings. The optimal settings for the standard Yandex Games platform will not be set.\nTo install them: change the platform to WebGL, enable the Auto Apply Settings option in the plugin settings and switch the platform in the Platforms field.",
                        "Ok");

                    NullPlatform();
#endif
                    void NullPlatform()
                    {
                        instance.Basic.platform = null;
                        instance.Basic.autoApplySettings = false;
                        instance.Basic.archivingBuild = false;
                        SetPlatform();
                        CompilationPipeline.RequestScriptCompilation();
                    }
                }
#else
                if (infoRes == null)
                    Debug.LogError($"{NAME_INFOYG_FILE} not found!");
#endif
                instance = infoRes;
            }

            return instance;
        }

        public ProjectSettings platformOptions { get => Basic.platform.projectSettings; }
#if UNITY_EDITOR
        public static void SetDefaultPlatform()
        {
            string standartPlatformSettingsPath = $"{PATCH_ASSETS_PLATFORMS}/YandexGames/YandexGames.asset";
            PlatformSettings standartPlatformSettings = AssetDatabase.LoadAssetAtPath<PlatformSettings>(standartPlatformSettingsPath);

            if (standartPlatformSettings != null)
            {
                instance.Basic.platform = standartPlatformSettings;

                EditorUtility.SetDirty(instance);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (YG2.infoYG.Basic.autoApplySettings)
                    instance.Basic.platform.ApplyProjectSettings();

                EditorScr.DefineSymbols.PlatformDefineSymbols();
            }
        }

        public static void SetPlatform(string selectPlatform = null)
        {
            string[] platfFolders = Directory.GetDirectories(PATCH_PC_PLATFORMS);

            for (int i = 0; i < platfFolders.Length; i++)
            {
                string folder = Path.Combine(platfFolders[i], "SDK").Replace("\\", "/");
                if (!Directory.Exists(folder))
                    continue;

                string platformName = Path.GetFileName(platfFolders[i]) + "Platform";
                string asmdefPath = Path.Combine(folder, platformName + ".asmdef").Replace("\\", "/");

                bool shouldHaveAsmdef = !string.IsNullOrEmpty(selectPlatform) && platformName != selectPlatform;

                bool asmdefExists = File.Exists(asmdefPath);

                if (shouldHaveAsmdef)
                {
                    if (!asmdefExists)
                    {
                        string templatePath = $"{PATCH_PC_YG2}/Scripts/Platform/Editor/AsmdefPlatformCreate.txt";
                        string content = File.ReadAllText(templatePath);
                        content = content.Replace("___PLATFORM_NAME___", platformName);
                        File.WriteAllText(asmdefPath, content);
                    }
                }
                else
                {
                    if (asmdefExists)
                    {
                        File.Delete(asmdefPath);
                    }
                }
            }
        }
#endif
    }
}
#endif