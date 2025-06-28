using System;
using System.Collections.Generic;
using Tables;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RewardDropdownAttribute))]
public class RewardDropdownDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType != SerializedPropertyType.String) {
            EditorGUI.LabelField(position, label.text, "Use with string only");
            return;
        }

        // Собери значения из всех нужных enum
        List<string> options = new List<string>();
        options.Add(RewardUtils.COINS_KEY);
        options.AddRange(Enum.GetNames(typeof(Crop)));
        options.AddRange(Enum.GetNames(typeof(ToolBuff)));

        int currentIndex = Mathf.Max(0, options.IndexOf(property.stringValue));
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, options.ToArray());

        property.stringValue = options[selectedIndex];
    }
}