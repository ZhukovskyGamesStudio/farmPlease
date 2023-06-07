using System;
using System.Collections;
using DefaultNamespace.Abstract;
using UnityEngine;

public class Settings : PreloadableSingleton<Settings> {

    public SettingsPanel SettingsPanel;
    public SettingsProfile settingsProfile;

    /**********/

    protected override void OnFirstInit() {
        settingsProfile = new SettingsProfile();
        settingsProfile.Load();
        LoadSettings();
    }

    public void LoadSettings() {
        Gps.Instance.InitializeNotifications();

        SettingsPanel.Initialize();
        SettingsPanel.UpdateSettingsPanel(settingsProfile);

        RealTImeManager.ChangeDayPoint(GetDayPoint().TotalSeconds, false);
        RealTImeManager.skipOne = settingsProfile.skipOne;
        if (Time.Instance != null)
            Time.Instance.ChangeDayPoint(GetDayPoint());
        //DebugManager.instance.Log("New day notify is " + settingsProfile.sendNotifications);
        Gps.Instance.NewDayNotification(settingsProfile.sendNotifications);

        Audio.Instance.ChangeVolume(settingsProfile.masterVolume, settingsProfile.musicVolume,
            settingsProfile.effectsVolume);
    }

    /**********/

    public void ChangeSettings(SettingsProfile profile) {
        settingsProfile = new SettingsProfile(profile);
        settingsProfile.Save();
    }

    public void NotificationChanged() {
        Gps.Instance.NewDayNotification(settingsProfile.sendNotifications);
    }

    public void DayMomentChanged() {
        settingsProfile.skipOne = true;
        settingsProfile.Save();

        RealTImeManager.ChangeDayPoint(GetDayPoint().TotalSeconds, true);
        if (Time.Instance)
            Time.Instance.ChangeDayPoint(GetDayPoint());
        Gps.Instance.NewDayNotification(settingsProfile.sendNotifications);
    }

    /**********/

    public TimeSpan GetDayPoint() {
        switch (settingsProfile.newDayPoint) {
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

public class SettingsProfile {
    public float effectsVolume;
    public float masterVolume;
    public float musicVolume;
    public int newDayPoint;
    public bool sendNotifications;
    public bool skipOne;

    public SettingsProfile() {
    }

    public SettingsProfile(SettingsProfile profile) {
        masterVolume = profile.masterVolume;
        musicVolume = profile.musicVolume;
        effectsVolume = profile.effectsVolume;
        sendNotifications = profile.sendNotifications;
        newDayPoint = profile.newDayPoint;
        skipOne = profile.skipOne;
    }

    public void Save() {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("effectsVolume", effectsVolume);
        PlayerPrefs.SetInt("sendNotifications", sendNotifications ? 1 : 0);
        PlayerPrefs.SetInt("newDayPoint", newDayPoint);
        PlayerPrefs.SetInt("skipOne", skipOne ? 1 : 0);
    }

    public void Load() {
        masterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        effectsVolume = PlayerPrefs.GetFloat("effectsVolume", 0.5f);
        sendNotifications = PlayerPrefs.GetInt("sendNotifications", 1) == 1;
        newDayPoint = PlayerPrefs.GetInt("newDayPoint", 2);
        skipOne = PlayerPrefs.GetInt("skipOne", 0) == 1;
    }

    public void Clear() {
        PlayerPrefs.DeleteKey("masterVolume");
        PlayerPrefs.DeleteKey("musicVolume");
        PlayerPrefs.DeleteKey("effectsVolume");
        PlayerPrefs.DeleteKey("sendNotifications");
        PlayerPrefs.DeleteKey("newDayPoint");
        PlayerPrefs.DeleteKey("skipOne");
    }
}