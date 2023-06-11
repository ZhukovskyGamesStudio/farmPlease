using UnityEngine;

namespace DefaultNamespace.Tables {
    [CreateAssetMenu(fileName = "SpotlightAnim", menuName = "ScriptableObjects/SpotlightAnimConfig", order = 7)]
    public class SpotlightAnimConfig : ScriptableObject {
        public Vector2 SpotlightSize;
        public Vector2 HeadShift;

        [Multiline]
        public string HintText;
    }
}