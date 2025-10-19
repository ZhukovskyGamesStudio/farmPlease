using System.Collections.Generic;
using UnityEngine;
using ZG_Localization;

public abstract class ConfigWithCroponomPage : ScriptableObject {
    [Header("CroponomPage")]
    public Sprite gridIcon;

    [field: SerializeField]
    public Sprite LockedGridIcon { get; private set; }

    [LocalizationKey("Croponom")]
    public string HeaderLoc;

    [LocalizationKey("Croponom")]
    public string FirstTextLoc;

    [LocalizationKey("Croponom")]
    public string SecondTextLoc;

    [field: SerializeField]
    public List<Sprite> FactsSprites { get; private set; }

    public abstract string GetUnlockable();

    public abstract int GetPageIndex();
}