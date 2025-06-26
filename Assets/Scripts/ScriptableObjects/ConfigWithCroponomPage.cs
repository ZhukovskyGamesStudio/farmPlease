using UnityEngine;

    public abstract class ConfigWithCroponomPage : ScriptableObject {
        [Header("CroponomPage")]
        public Sprite gridIcon;

        [field: SerializeField]
        public Sprite LockedGridIcon { get; private set; }

        public string header;

        [TextArea]
        public string firstText;
        [LocalizationKey("Croponom")]
        public string FirstTextLoc;

        public Sprite firstSprite;

        [TextArea]
        public string secondText;
        [LocalizationKey("Croponom")]
        public string SecondTextLoc;

        public Sprite secondSprite;
        
        public abstract string GetUnlockable();
    }
