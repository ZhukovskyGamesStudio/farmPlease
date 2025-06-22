using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

public class TMPRemoveLigatureTableWindow : EditorWindow
{
    TMP_FontAsset fontAsset;

    [MenuItem("Tools/TMP Font - Очистить лигатуры (Font Feature Table)")]
    public static void ShowWindow()
    {
        GetWindow<TMPRemoveLigatureTableWindow>("Очистка лигатур TMP");
    }

    void OnGUI()
    {
        GUILayout.Label("Удалить лигатуры из Font Feature Table", EditorStyles.boldLabel);
        fontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font Asset", fontAsset, typeof(TMP_FontAsset), false);

        GUI.enabled = fontAsset != null;

        if (GUILayout.Button("Удалить лигатуры"))
        {
            RemoveLigaturesFromFontAsset(fontAsset);
        }

        GUI.enabled = true;
    }

    void RemoveLigaturesFromFontAsset(TMP_FontAsset fontAsset)
    {
        if (fontAsset.fontFeatureTable == null)
        {
            Debug.LogWarning($"[TMP] У шрифта {fontAsset.name} нет Font Feature Table.");
            return;
        }

        int countBefore = fontAsset.fontFeatureTable.ligatureRecords?.Count ?? 0;

        if (countBefore == 0)
        {
            Debug.Log($"[TMP] У шрифта {fontAsset.name} уже нет лигатур.");
            return;
        }

        Undo.RecordObject(fontAsset, "Clear TMP Ligatures");
        fontAsset.fontFeatureTable.ligatureRecords?.Clear();
        
        Debug.Log($"[TMP] Удалено {countBefore} лигатур из шрифта {fontAsset.name}.");
        
         countBefore = fontAsset.fontFeatureTable.glyphPairAdjustmentRecords?.Count ?? 0;

        if (countBefore == 0)
        {
            Debug.Log($"[TMP] У шрифта {fontAsset.name} уже нет glyphPairAdjustment.");
            return;
        }

        Undo.RecordObject(fontAsset, "Clear TMP glyphPairAdjustmentRecords");
        fontAsset.fontFeatureTable.glyphPairAdjustmentRecords?.Clear();
        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();

        Debug.Log($"[TMP] Удалено {countBefore} glyphPairAdjustment из шрифта {fontAsset.name}.");
    }
}