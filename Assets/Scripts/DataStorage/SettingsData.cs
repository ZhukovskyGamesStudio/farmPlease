using System;

[Serializable]
public class SettingsData {
    public float EffectsVolume = 0.5f;
    public float MasterVolume = 1f;
    public float MusicVolume = 0.5f;
    public int NewDayPoint = 2;
    public bool SendNotifications = true;
    public bool SkipOne = false;

    public SettingsData() { }

    public SettingsData(SettingsData data) {
        MasterVolume = data.MasterVolume;
        MusicVolume = data.MusicVolume;
        EffectsVolume = data.EffectsVolume;
        SendNotifications = data.SendNotifications;
        NewDayPoint = data.NewDayPoint;
        SkipOne = data.SkipOne;
    }
}