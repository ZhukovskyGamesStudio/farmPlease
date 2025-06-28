using UnityEditor;
using UnityEngine;
using System.Linq;
using Localization;

[CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
public class LocalizationKeyDrawer : PropertyDrawer
{
    private static string _lastFileName;
    private static string[] _keys;

    private static void LoadKeys(string fileName)
    {
        var keys = LocalizationUtils.GetKeysForFile(fileName);
        _keys = keys.ToArray();
        _lastFileName = fileName;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use with string only");
            return;
        }

        var attr = (LocalizationKeyAttribute)attribute;
        if (_lastFileName != attr.fileName)
            LoadKeys(attr.fileName);
        int currentIndex = Mathf.Max(0, System.Array.IndexOf(_keys, property.stringValue));
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, _keys);

        if (selectedIndex >= 0 && selectedIndex < _keys.Length)
            property.stringValue = _keys[selectedIndex];
    }
} 