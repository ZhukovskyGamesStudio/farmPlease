using Lofelt.NiceVibrations;
using UnityEngine;

public static class VibrationsUtils {
    public static void SetVibrations(bool isOn) {
        HapticController.hapticsEnabled = isOn;
    }
}