using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Devlog", menuName = "Scriptable Objects/Devlog", order = 3)]
    public class DevlogConfig : ScriptableObject {
        public string Header;
    
        [TextArea(15,20)]
        public string MainText;
    }
}