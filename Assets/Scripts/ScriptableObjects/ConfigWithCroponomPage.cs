using System.Collections.Generic;
using UnityEngine;
using ZG_Localization;
    public abstract class ConfigWithCroponomPage : ScriptableObject {
        [Header("CroponomPage")]
        public Sprite gridIcon;

        [field: SerializeField]
        public Sprite LockedGridIcon { get; private set; }

        public string header;

        [LocalizationKey("Croponom")]
        public string HeaderLoc;

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
        
        
        [field: SerializeField]
        public List<Sprite> FactsSprites { get; private set; }
        
        public abstract string GetUnlockable();

        public abstract int GetPageIndex();
    }
