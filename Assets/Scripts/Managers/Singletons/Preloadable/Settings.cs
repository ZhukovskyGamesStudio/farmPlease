using System;
using Dialogs;
using Managers;
using UI;
using ZhukovskyGamesPlugin;

public class Settings : Singleton<Settings> {
    public SettingsData SettingsData => SaveLoadManager.CurrentSave.SettingsData;

    public void OpenSettings() {
        DialogsManager.Instance.ShowDialogWithData(typeof(SettingsDialog), ConfigsManager.Instance.CheatCodeConfigList);
    }

    public void InitSettingsView() {
        RealTImeManager.ChangeDayPoint(GetDayPoint().TotalSeconds, false);
        RealTImeManager.SkipOne = SettingsData.SkipOne;
        Audio.Instance.ChangeVolume(SettingsData.MasterVolume, SettingsData.MusicVolume, SettingsData.EffectsVolume);
    }

    public void ChangeSettings(SettingsData data) {
        SaveLoadManager.CurrentSave.SettingsData = new SettingsData(data);
        SaveLoadManager.SaveGame();
    }

    public void NotificationChanged() { }

    public TimeSpan GetDayPoint() {
        switch (SettingsData.NewDayPoint) {
            case 0:
                return new TimeSpan(0, 0, 0);

            case 1:
                return new TimeSpan(6, 0, 0);

            case 2:
                return new TimeSpan(12, 0, 0);

            case 3:
                return new TimeSpan(18, 0, 0);
        }

        UnityEngine.Debug.Log("error");
        return new TimeSpan(12, 0, 0);
    }
}