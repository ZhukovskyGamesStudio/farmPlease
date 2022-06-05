using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public SettingsPanel SettingsPanel;
    public SettingsProfile settingsProfile;

    public static SettingsManager instance;

    /**********/

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            settingsProfile = new SettingsProfile();
            settingsProfile.Load();
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        LoadSettings();
        AudioManager.instance.Initialize();
    }

    public void LoadSettings()
    {
       
        GPSManager.instance.InitializeNotifications();

        SettingsPanel.Initialize();
        SettingsPanel.UpdateSettingsPanel(settingsProfile);

        RealTImeManager.ChangeDayPoint(GetDayPoint().TotalSeconds, false);
        RealTImeManager.skipOne = settingsProfile.skipOne;
        if (TimeManager.instance != null)
            TimeManager.instance.ChangeDayPoint(GetDayPoint());
        //DebugManager.instance.Log("New day notify is " + settingsProfile.sendNotifications);
        GPSManager.instance.NewDayNotification(settingsProfile.sendNotifications);

        AudioManager.instance.ChangeVolume(settingsProfile.masterVolume, settingsProfile.musicVolume, settingsProfile.effectsVolume);

    }


    /**********/


    public void ChangeSettings(SettingsProfile profile)
    {
        settingsProfile = new SettingsProfile(profile);
        settingsProfile.Save();
    }

    public void NotificationChanged()
    {
        GPSManager.instance.NewDayNotification(settingsProfile.sendNotifications);
    }

    public void DayMomentChanged()
    {
        settingsProfile.skipOne = true;
        settingsProfile.Save();


        RealTImeManager.ChangeDayPoint(GetDayPoint().TotalSeconds, true);
        if (TimeManager.instance)
            TimeManager.instance.ChangeDayPoint(GetDayPoint());
        GPSManager.instance.NewDayNotification(settingsProfile.sendNotifications);
    }



    /**********/

    public TimeSpan GetDayPoint()
    {
        switch (settingsProfile.newDayPoint)
        {
            case 0:
                return new TimeSpan(hours: 0, 0, 0);
            case 1:
                return new TimeSpan(hours: 6, 0, 0);
            case 2:
                return new TimeSpan(hours: 12, 0, 0);
            case 3:
                return new TimeSpan(hours: 18, 0, 0);
        }

        Debug.Log("error");
        return new TimeSpan(hours: 12, 0, 0);

    }
}

public class SettingsProfile
{
    public float masterVolume;
    public float musicVolume;
    public float effectsVolume;
    public bool sendNotifications;
    public int newDayPoint;
    public bool skipOne;

    public SettingsProfile() { }

    public SettingsProfile(SettingsProfile profile)
    {
        masterVolume = profile.masterVolume;
        musicVolume = profile.musicVolume;
        effectsVolume = profile.effectsVolume;
        sendNotifications = profile.sendNotifications;
        newDayPoint = profile.newDayPoint;
        skipOne = profile.skipOne;

    }

    public void Save()
    {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("effectsVolume", effectsVolume);
        PlayerPrefs.SetInt("sendNotifications", sendNotifications ? 1 : 0);
        PlayerPrefs.SetInt("newDayPoint", newDayPoint);
        PlayerPrefs.SetInt("skipOne", skipOne ? 1 : 0);
    }

    public void Load()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        effectsVolume = PlayerPrefs.GetFloat("effectsVolume", 0.5f);
        sendNotifications = PlayerPrefs.GetInt("sendNotifications", 1) == 1;
        newDayPoint = PlayerPrefs.GetInt("newDayPoint", 2);
        skipOne = PlayerPrefs.GetInt("skipOne", 0) == 1;
    }

    public void Clear()
    {
        PlayerPrefs.DeleteKey("masterVolume");
        PlayerPrefs.DeleteKey("musicVolume");
        PlayerPrefs.DeleteKey("effectsVolume");
        PlayerPrefs.DeleteKey("sendNotifications");
        PlayerPrefs.DeleteKey("newDayPoint");
        PlayerPrefs.DeleteKey("skipOne");
    }


}


