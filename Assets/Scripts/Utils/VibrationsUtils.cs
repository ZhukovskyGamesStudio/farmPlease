using Lofelt.NiceVibrations;
using Managers;

public static class VibrationsUtils {
    public static void SetVibrations(bool isOn) {
        HapticController.outputLevel = isOn ? 1 : 0;
    }

    public static void Vibrate(HapticPatterns.PresetType preset) {
        if (!SaveLoadManager.CurrentSave.SettingsData.Vibrations) {
            return;
        }

        HapticPatterns.PlayPreset(preset);
    }
}