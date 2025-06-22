using UnityEngine;
using UnityEditor;
using TMPro;

public class TMPFontReplacerWindow : EditorWindow
{
    TMP_FontAsset oldFont;
    TMP_FontAsset newFont;

    [MenuItem("Tools/Replace TMP Font (Drag & Drop)")]
    public static void ShowWindow()
    {
        GetWindow<TMPFontReplacerWindow>("TMP Font Replacer");
    }

    void OnGUI()
    {
        GUILayout.Label("Выбор шрифтов", EditorStyles.boldLabel);

        oldFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Старый шрифт", oldFont, typeof(TMP_FontAsset), false);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Новый шрифт", newFont, typeof(TMP_FontAsset), false);

        GUI.enabled = oldFont != null && newFont != null;

        if (GUILayout.Button("Заменить во всех префабах и сценах"))
        {
            if (EditorUtility.DisplayDialog("Подтверждение", "Заменить все вхождения TMP шрифта?", "Да", "Отмена"))
            {
                ReplaceFontInProject(oldFont, newFont);
            }
        }

        GUI.enabled = true;
    }

    static void ReplaceFontInProject(TMP_FontAsset oldFont, TMP_FontAsset newFont)
    {
        string[] allGuids = AssetDatabase.FindAssets("t:Prefab t:Scene");

        foreach (string guid in allGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj == null) continue;

            bool changed = false;

            // Заменяем в TextMeshProUGUI
            foreach (var tmp in obj.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (tmp.font == oldFont)
                {
                    tmp.font = newFont;
                    changed = true;
                }
            }

            // Заменяем в TextMeshPro (3D)
            foreach (var tmp in obj.GetComponentsInChildren<TextMeshPro>(true))
            {
                if (tmp.font == oldFont)
                {
                    tmp.font = newFont;
                    changed = true;
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(obj);
                Debug.Log($"[FontReplacer] Заменён шрифт в: {path}");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[FontReplacer] Готово.");
    }
}
