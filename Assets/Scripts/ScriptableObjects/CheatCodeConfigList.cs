using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "CheatCodeList", menuName = "Scriptable Objects/Lists/CheatCodeList", order = 0)]
    public class CheatCodeConfigList : ScriptableObject {
        public List<CheatCodeConfig> CheatCodes;
    }
}