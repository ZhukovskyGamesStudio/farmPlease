using Managers;
using UnityEditor;
using UnityEngine;

public class EditorMethods : MonoBehaviour
{
    
#if UNITY_EDITOR 
    [MenuItem("FarmPlease Tools/ClearSave")]
    private static void ClearSave() {
       SaveLoadManager.ClearSave();
    }

#endif
  
}
