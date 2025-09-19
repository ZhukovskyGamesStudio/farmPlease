using Managers;

public class LocalizationManager : ZG_Localization.LocalizationManager {
    public override string LoadCurrentLanguage() {
        return SaveLoadManager.CurrentSave.CurrentLanguage;
    }

    public override void SaveCurrentLanguage(string value) {
        SaveLoadManager.CurrentSave.CurrentLanguage = value;
    }
}