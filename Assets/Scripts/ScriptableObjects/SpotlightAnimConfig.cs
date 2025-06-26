using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "SpotlightAnim", menuName = "Scriptable Objects/SpotlightAnimConfig", order = 7)]
    public class SpotlightAnimConfig : ScriptableObject {
        public Vector2 SpotlightSize;
        public Vector2 HeadShift;

        [Multiline]
        public string HintText;
        [LocalizationKey("Ftue")]
        public string HintTextLoc;
    }
}