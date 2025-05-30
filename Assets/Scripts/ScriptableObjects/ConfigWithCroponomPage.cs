using UnityEngine;

    public abstract class ConfigWithCroponomPage : ScriptableObject {
        [Header("CroponomPage")]
        public Sprite gridIcon;

        [field: SerializeField]
        public Sprite LockedGridIcon { get; private set; }

        public string header;

        [TextArea]
        public string firstText;

        public Sprite firstSprite;

        [TextArea]
        public string secondText;

        public Sprite secondSprite;
        
        public abstract string GetUnlockable();
    }
