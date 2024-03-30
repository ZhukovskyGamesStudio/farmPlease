using UnityEngine;

namespace ScriptableObjects
{
    public abstract class ConfigWithCroponomPage : ScriptableObject {
        [Header("CroponomPage")]
        public Sprite gridIcon;
        public string header;
        [TextArea]
        public string firstText;
        public Sprite firstSprite;
        [TextArea]
        public string secondText;
        public Sprite secondSprite;
    }
}