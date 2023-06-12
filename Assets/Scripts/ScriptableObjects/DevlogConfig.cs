using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Devlog", menuName = "ScriptableObjects/Devlog", order = 3)]
public class DevlogConfig : ScriptableObject {
    public string Header;
    
    [TextArea(15,20)]
    public string MainText;
}